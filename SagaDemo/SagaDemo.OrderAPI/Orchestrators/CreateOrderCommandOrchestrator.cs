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
            var transactionId = guidProvider.GenerateGuidString();
            var userId = command.UserId;

            await CreateDocumentIfNotExistsAsync(transactionId, cancellationToken).ConfigureAwait(false);

            // TODO: Create an endpoint in the inventory api to get the cost.
            var totalCost = 100;

            await ConsumeLoyaltyPointsAsync(transactionId, totalCost, userId, cancellationToken).ConfigureAwait(false);

            var reservationsRequest = ReservationsMapper.ToReservationsApiContract(command.Order);

            await reservationsApiClient.ReserveItemsAsync(transactionId, reservationsRequest, cancellationToken).ConfigureAwait(false);

            var address = AddressMapper.ToDeliveryApiContract(command.Address);

            await deliveryApiClient.CreateDeliveryRequestAsync(transactionId, address, cancellationToken).ConfigureAwait(false);

            throw new NotImplementedException();
        }

        private async Task CreateDocumentIfNotExistsAsync(string transactionId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                if (transactionDocument == null)
                {
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
        }

        private async Task ConsumeLoyaltyPointsAsync(string transactionId, int totalCost, int userId, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(session, transactionId);
                var transactionDocument = await session.LoadAsync<OrderTransaction>(transactionDocumentId, cancellationToken).ConfigureAwait(false);

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.InProgress;

                try
                {
                    await loyaltyPointsApiClient.ConsumePointsAsync(transactionId, new ConsumePointsRequest(totalCost, userId), cancellationToken).ConfigureAwait(false);

                    transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.Completed;
                }
                catch (SwaggerException<LoyaltyPointsAPI.ApiClient.ValidationProblemDetails> validationException)
                {
                    // Permanent error
                    // TODO: Roll back all other steps. Will have to track whether a step has been undone (or rely on idempotence of rollback operations).

                    transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus = StepStatus.RolledBack;
                }
                catch
                {
                    ++transactionDocument.LoyaltyPointsConsumptionStepDetails.Attempts;
                }

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
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
    }
}