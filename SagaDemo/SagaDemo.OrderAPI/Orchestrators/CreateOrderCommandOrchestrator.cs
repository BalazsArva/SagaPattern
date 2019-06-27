using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.AspNetCore;
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
    public class CreateOrderCommandOrchestrator
    {
        private const int MaxAttemptsPerStep = 10;

        private readonly IGuidProvider guidProvider;
        private readonly IDocumentStore documentStore;
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
        {
            this.guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
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

            await CreateTransactionDocumentIfNotExistsAsync(transactionId, cancellationToken).ConfigureAwait(false);

            // TODO: Consider batching this somehow.
            // TODO: Error handling for the API call. (Can use Polly policies once they are set up.)
            foreach (var orderItem in command.Order.Items)
            {
                var product = await catalogApiClient.GetItemAsync(orderItem.ProductId, cancellationToken).ConfigureAwait(false);

                totalCost += product.Result.PointsCost;
            }

            var consumeLoyaltyPointsResult = await ConsumeLoyaltyPointsAsync(transactionId, totalCost, userId, cancellationToken).ConfigureAwait(false);
            if (consumeLoyaltyPointsResult == StepResult.Abort)
            {
                // TODO: Will have to do something about interrupted rollbacks as well (probably the best would be to store pending operations in a queue or periodically poll unfinished orders)
                await RollbackAsync(transactionId, cancellationToken).ConfigureAwait(false);
            }
            // TODO: Handle Retry case

            var reserveItemsResult = await ReserveItemsAsync(transactionId, command, cancellationToken).ConfigureAwait(false);
            if (reserveItemsResult == StepResult.Abort)
            {
                // TODO: Will have to do something about interrupted rollbacks as well (probably the best would be to store pending operations in a queue or periodically poll unfinished orders)
                await RollbackAsync(transactionId, cancellationToken).ConfigureAwait(false);
            }
            // TODO: Handle Retry case

            var createDeliveryResult = await CreateDeliveryRequestAsync(transactionId, command, cancellationToken).ConfigureAwait(false);
            if (createDeliveryResult == StepResult.Abort)
            {
                // TODO: Will have to do something about interrupted rollbacks as well (probably the best would be to store pending operations in a queue or periodically poll unfinished orders)
                await RollbackAsync(transactionId, cancellationToken).ConfigureAwait(false);
            }
            // TODO: Handle Retry case

            // TODO: Finalize transaction
        }

        private async Task CreateTransactionDocumentIfNotExistsAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                if (transactionDocument != null)
                {
                    return;
                }

                transactionDocument = new OrderTransaction
                {
                    Id = transactionDocumentId,
                    TransactionStatus = TransactionStatus.NotStarted,
                    LoyaltyPointsConsumptionStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        StepStatus = StepStatus.NotStarted
                    },
                    DeliveryCreationStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        StepStatus = StepStatus.NotStarted
                    },
                    InventoryReservationStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        StepStatus = StepStatus.NotStarted
                    }
                };

                await session.StoreAsync(transactionDocument, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task<StepResult> ConsumeLoyaltyPointsAsync(string transactionId, int totalCost, int userId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                StepResult result;

                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.InProgress;

                try
                {
                    await loyaltyPointsApiClient.ConsumePointsAsync(transactionId, new ConsumePointsRequest(totalCost, userId), cancellationToken).ConfigureAwait(false);

                    transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.Completed;
                    result = StepResult.Successful;
                }
                catch (SwaggerException<LoyaltyPointsAPI.ApiClient.ValidationProblemDetails>)
                {
                    // Permanent error
                    transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.RolledBack;
                    transactionDocument.TransactionStatus = TransactionStatus.PermanentFailure;

                    result = StepResult.Abort;
                }
                catch
                {
                    ++transactionDocument.LoyaltyPointsConsumptionStepDetails.Attempts;

                    result = transactionDocument.LoyaltyPointsConsumptionStepDetails.Attempts > MaxAttemptsPerStep
                        ? StepResult.Abort
                        : StepResult.Retry;
                }

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);

                return result;
            }
        }

        private async Task<StepResult> ReserveItemsAsync(string transactionId, CreateOrderCommand command, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                StepResult result;

                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.InventoryReservationStepDetails.StepStatus = StepStatus.InProgress;

                try
                {
                    var reservationsRequest = ReservationsMapper.ToReservationsApiContract(command.Order);

                    await reservationsApiClient.ReserveItemsAsync(transactionId, reservationsRequest, cancellationToken).ConfigureAwait(false);

                    transactionDocument.InventoryReservationStepDetails.StepStatus = StepStatus.Completed;
                    result = StepResult.Successful;
                }
                catch (SwaggerException<InventoryAPI.ApiClient.ValidationProblemDetails>)
                {
                    // Permanent error. Can set step state to rolled back since if a validation error occurs, then no
                    // change has occured in the inventory API so there is nothing to roll back.
                    transactionDocument.InventoryReservationStepDetails.StepStatus = StepStatus.RolledBack;
                    transactionDocument.TransactionStatus = TransactionStatus.PermanentFailure;

                    result = StepResult.Abort;
                }
                catch
                {
                    ++transactionDocument.InventoryReservationStepDetails.Attempts;

                    result = transactionDocument.InventoryReservationStepDetails.Attempts > MaxAttemptsPerStep
                        ? StepResult.Abort
                        : StepResult.Retry;
                }

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);

                return result;
            }
        }

        private async Task<StepResult> CreateDeliveryRequestAsync(string transactionId, CreateOrderCommand command, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                StepResult result;

                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.DeliveryCreationStepDetails.StepStatus = StepStatus.InProgress;

                try
                {
                    var address = AddressMapper.ToDeliveryApiContract(command.Address);

                    await deliveryApiClient.CreateDeliveryRequestAsync(transactionId, address, cancellationToken).ConfigureAwait(false);

                    transactionDocument.DeliveryCreationStepDetails.StepStatus = StepStatus.Completed;
                    result = StepResult.Successful;
                }
                catch (SwaggerException<DeliveryAPI.ApiClient.ValidationProblemDetails>)
                {
                    transactionDocument.DeliveryCreationStepDetails.StepStatus = StepStatus.RolledBack;
                    transactionDocument.TransactionStatus = TransactionStatus.PermanentFailure;

                    result = StepResult.Abort;
                }
                catch
                {
                    ++transactionDocument.DeliveryCreationStepDetails.Attempts;

                    result = transactionDocument.DeliveryCreationStepDetails.Attempts > MaxAttemptsPerStep
                        ? StepResult.Abort
                        : StepResult.Retry;
                }

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);

                return result;
            }
        }

        private async Task RollbackAsync(string transactionId, CancellationToken cancellationToken)
        {
            // TODO: Retry strategy
            await RollbackLoyaltyPointsConsumptionAsync(transactionId, cancellationToken).ConfigureAwait(false);
            await RollbackInventoryReservationAsync(transactionId, cancellationToken).ConfigureAwait(false);
            await RollbackDeliveryCreationAsync(transactionId, cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                if (transactionDocument.TransactionStatus == TransactionStatus.RolledBack)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.TransactionStatus = TransactionStatus.RolledBack;

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task RollbackLoyaltyPointsConsumptionAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var stepStatus = transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus;
                if (stepStatus == StepStatus.RolledBack || stepStatus == StepStatus.NotStarted)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                // TODO: Handle errors. E.g. no corresponsing consumption event found can be ignored.
                await loyaltyPointsApiClient.RefundPointsAsync(transactionId, cancellationToken).ConfigureAwait(false);

                transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.RolledBack;

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task RollbackInventoryReservationAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var stepStatus = transactionDocument.InventoryReservationStepDetails.StepStatus;
                if (stepStatus == StepStatus.RolledBack || stepStatus == StepStatus.NotStarted)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                // TODO: Perform actual rollback.
                // Will have to create an endpoint in the inventory API.

                transactionDocument.InventoryReservationStepDetails.StepStatus = StepStatus.RolledBack;

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private async Task RollbackDeliveryCreationAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var stepStatus = transactionDocument.DeliveryCreationStepDetails.StepStatus;
                if (stepStatus == StepStatus.RolledBack || stepStatus == StepStatus.NotStarted)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                var deliveryDetails = await deliveryApiClient.GetDeliveryDetailsAsync(transactionId, cancellationToken).ConfigureAwait(false);
                var entityVersion = deliveryDetails.Headers[CustomHttpHeaderKeys.EntityVersion].Single();

                // TODO: Handle concurrency and other errors
                await deliveryApiClient.CancelDeliveryAsync(transactionId, entityVersion, cancellationToken).ConfigureAwait(false);

                transactionDocument.DeliveryCreationStepDetails.StepStatus = StepStatus.RolledBack;

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }

        private enum StepResult
        {
            Successful,

            Retry,

            Abort
        }
    }
}