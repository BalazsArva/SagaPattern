using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.AspNetCore;
using SagaDemo.Common.DataAccess.RavenDb.Extensions;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.Common.Errors.Swagger;
using SagaDemo.DeliveryAPI.ApiClient;
using SagaDemo.InventoryAPI.ApiClient;
using SagaDemo.LoyaltyPointsAPI.ApiClient;
using SagaDemo.OrderAPI.Entitites;
using SagaDemo.OrderAPI.Mappers;
using SagaDemo.OrderAPI.Operations.Commands;
using SagaDemo.OrderAPI.Providers;

namespace SagaDemo.OrderAPI.Orchestrators
{
    public class CreateOrderCommandOrchestrator : TransactionOrchestratorBase<OrderTransaction>, ICreateOrderCommandOrchestrator
    {
        private const int BadRequestStatusCode = 400;
        private const int ConflictStatusCode = 409;

        private readonly IGuidProvider guidProvider;
        private readonly ILoyaltyPointsApiClient loyaltyPointsApiClient;
        private readonly ICatalogApiClient catalogApiClient;
        private readonly IReservationsApiClient reservationsApiClient;
        private readonly IDeliveryApiClient deliveryApiClient;

        public CreateOrderCommandOrchestrator(
            IGuidProvider guidProvider,
            IDocumentStore documentStore,
            ILoyaltyPointsApiClient loyaltyPointsApiClient,
            ICatalogApiClient catalogApiClient,
            IReservationsApiClient reservationsApiClient,
            IDeliveryApiClient deliveryApiClient)
            : base(documentStore)
        {
            this.guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            this.loyaltyPointsApiClient = loyaltyPointsApiClient ?? throw new ArgumentNullException(nameof(loyaltyPointsApiClient));
            this.catalogApiClient = catalogApiClient ?? throw new ArgumentNullException(nameof(catalogApiClient));
            this.deliveryApiClient = deliveryApiClient ?? throw new ArgumentNullException(nameof(deliveryApiClient));
            this.reservationsApiClient = reservationsApiClient ?? throw new ArgumentNullException(nameof(reservationsApiClient));
        }

        public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var totalCost = 0;
            var transactionId = guidProvider.GenerateGuidString();
            var userId = command.UserId;

            // TODO: Consider batching this somehow.
            // TODO: Error handling for the API call.
            foreach (var orderItem in command.Order.Items)
            {
                var product = await catalogApiClient.GetItemAsync(orderItem.ProductId, cancellationToken).ConfigureAwait(false);

                totalCost += product.Result.PointsCost;
            }

            // TODO: Will have to do something about interrupted rollbacks as well (probably the best would be to store pending operations in a queue or periodically poll unfinished orders)
            await ConsumeLoyaltyPointsAsync(transactionId, totalCost, userId, cancellationToken).ConfigureAwait(false);
            await ReserveItemsAsync(transactionId, command, cancellationToken).ConfigureAwait(false);
            await CreateDeliveryRequestAsync(transactionId, command, cancellationToken).ConfigureAwait(false);

            var transactionStatus = await GetTransactionStatusAsync(transactionId, cancellationToken).ConfigureAwait(false);
            if (transactionStatus == TransactionStatus.PermanentFailure)
            {
                await RollbackAsync(transactionId, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await FinalizeTransactionAsync(transactionId, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task RollbackAsync(string transactionId, CancellationToken cancellationToken)
        {
            await RollbackLoyaltyPointsConsumptionAsync(transactionId, cancellationToken).ConfigureAwait(false);
            await RollbackInventoryReservationAsync(transactionId, cancellationToken).ConfigureAwait(false);
            await RollbackDeliveryCreationAsync(transactionId, cancellationToken).ConfigureAwait(false);

            await DocumentStore
                .ExecuteInConcurrentSessionAsync(async (session, ct) =>
                {
                    var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                    var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, ct).ConfigureAwait(false);

                    if (transactionDocument.TransactionStatus == TransactionStatus.RolledBack)
                    {
                        return;
                    }

                    var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                    transactionDocument.TransactionStatus = TransactionStatus.RolledBack;

                    await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, ct).ConfigureAwait(false);
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }, cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ConsumeLoyaltyPointsAsync(string transactionId, int totalCost, int userId, CancellationToken cancellationToken)
        {
            await PerformTransactionStepAsync(
                transactionId,
                t => t.LoyaltyPointsConsumptionStepDetails,
                async ct =>
                {
                    try
                    {
                        await loyaltyPointsApiClient.ConsumePointsAsync(transactionId, new ConsumePointsRequest(totalCost, userId), ct).ConfigureAwait(false);

                        return StepResult.Successful;
                    }
                    catch (SwaggerException<LoyaltyPointsAPI.ApiClient.ValidationProblemDetails>)
                    {
                        return StepResult.Abort;
                    }
                },
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task ReserveItemsAsync(string transactionId, CreateOrderCommand command, CancellationToken cancellationToken)
        {
            await PerformTransactionStepAsync(
                transactionId,
                t => t.InventoryReservationStepDetails,
                async ct =>
                {
                    try
                    {
                        var reservationsRequest = ReservationsMapper.ToReservationsApiContract(command.Order);

                        await reservationsApiClient.ReserveItemsAsync(transactionId, reservationsRequest, ct).ConfigureAwait(false);

                        return StepResult.Successful;
                    }
                    catch (SwaggerException<InventoryAPI.ApiClient.ValidationProblemDetails>)
                    {
                        // Permanent error. Can set step state to rolled back since if a validation error occurs, then no
                        // change has occured in the inventory API so there is nothing to roll back.
                        return StepResult.Abort;
                    }
                },
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task CreateDeliveryRequestAsync(string transactionId, CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var address = AddressMapper.ToDeliveryApiContract(command.Address);

            await PerformTransactionStepAsync(
                transactionId,
                t => t.DeliveryCreationStepDetails,
                async ct =>
                {
                    try
                    {
                        var reservationsRequest = ReservationsMapper.ToReservationsApiContract(command.Order);

                        await deliveryApiClient.CreateDeliveryRequestAsync(transactionId, address, ct).ConfigureAwait(false);

                        return StepResult.Successful;
                    }
                    catch (SwaggerException<InventoryAPI.ApiClient.ValidationProblemDetails>)
                    {
                        return StepResult.Abort;
                    }
                },
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task RollbackLoyaltyPointsConsumptionAsync(string transactionId, CancellationToken cancellationToken)
        {
            await PerformRollbackStepAsync(
                transactionId,
                t => t.LoyaltyPointsConsumptionStepDetails,
                async ct => await loyaltyPointsApiClient.RefundPointsAsync(transactionId, ct).ConfigureAwait(false),
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task RollbackInventoryReservationAsync(string transactionId, CancellationToken cancellationToken)
        {
            await PerformRollbackStepAsync(
                transactionId,
                t => t.InventoryReservationStepDetails,
                async ct => await reservationsApiClient.CancelReservationAsync(transactionId, ct).ConfigureAwait(false),
                cancellationToken)
                .ConfigureAwait(false);
        }

        private async Task RollbackDeliveryCreationAsync(string transactionId, CancellationToken cancellationToken)
        {
            await PerformRollbackStepAsync(
                transactionId,
                t => t.InventoryReservationStepDetails,
                async ct =>
                {
                    var deliveryDetails = await deliveryApiClient.GetDeliveryDetailsAsync(transactionId, ct).ConfigureAwait(false);
                    var entityVersion = deliveryDetails.Headers[CustomHttpHeaderKeys.EntityVersion].Single();

                    if (deliveryDetails.Result.Status == DeliveryStatus.Cancelled)
                    {
                        return;
                    }

                    await deliveryApiClient.CancelDeliveryAsync(transactionId, entityVersion, ct).ConfigureAwait(false);
                },
                cancellationToken)
                .ConfigureAwait(false);
        }
    }
}