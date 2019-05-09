﻿using System;
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

namespace SagaDemo.OrderAPI.Orchestrators
{
    public class CreateOrderCommandOrchestrator
    {
        private readonly IDocumentStore documentStore;
        private readonly ILoyaltyPointsApiClient loyaltyPointsApiClient;
        private readonly ICatalogApiClient catalogApiClient;
        private readonly IDeliveryApiClient deliveryApiClient;

        public CreateOrderCommandOrchestrator(
            IDocumentStore documentStore,
            ILoyaltyPointsApiClient loyaltyPointsApiClient,
            ICatalogApiClient catalogApiClient,
            IDeliveryApiClient deliveryApiClient)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.loyaltyPointsApiClient = loyaltyPointsApiClient ?? throw new ArgumentNullException(nameof(loyaltyPointsApiClient));
            this.catalogApiClient = catalogApiClient ?? throw new ArgumentNullException(nameof(catalogApiClient));
            this.deliveryApiClient = deliveryApiClient ?? throw new ArgumentNullException(nameof(deliveryApiClient));
        }

        public async Task HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            // TODO: Move to a provider for testability
            var transactionId = Guid.NewGuid().ToString();
            var userId = command.UserId;

            await CreateDocumentIfNotExists(transactionId, cancellationToken).ConfigureAwait(false);

            // TODO: Create an endpoint in the inventory api to get the cost.
            var totalCost = 100;

            await ConsumeLoyaltyPointsAsync(transactionId, totalCost, userId, cancellationToken).ConfigureAwait(false);

            var reservationsRequest = ReservationsMapper.ToReservationsApiContract(command.Order);

            // TODO: The endpoint should accept transactionId
            await catalogApiClient.ReserveItemsAsync(reservationsRequest, cancellationToken).ConfigureAwait(false);

            var address = AddressMapper.ToDeliveryApiContract(command.Address);

            await deliveryApiClient.CreateDeliveryRequestAsync(transactionId, address, cancellationToken).ConfigureAwait(false);

            throw new NotImplementedException();
        }

        private async Task CreateDocumentIfNotExists(string transactionId, CancellationToken cancellationToken)
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

                try
                {
                    // TODO: Should create and endpoint which can refund points based on transactionId.
                    // This will require event sourcing-like solution, but makes it possible to detect multiple refund requests and handle them idempotently.
                    await loyaltyPointsApiClient.ConsumePointsAsync(new ConsumePointsCommand(totalCost, transactionId, userId), cancellationToken).ConfigureAwait(false);

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

                if (transactionDocument.LoyaltyPointsConsumptionStepDetails.StepStatus == StepStatus.RolledBack)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                // TODO: Perform actual rollback.
                // Will need to create a separate endpoint in the loyatly points API which figures out the amount based on tranactionId, detects duplicate attempts, etc.

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

                if (transactionDocument.InventoryReservationStepDetails.StepStatus == StepStatus.RolledBack)
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

                if (transactionDocument.DeliveryCreationStepDetails.StepStatus == StepStatus.RolledBack)
                {
                    return;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(transactionDocument);

                var deliveryDetails = await deliveryApiClient.GetDeliveryDetailsAsync(transactionId, cancellationToken).ConfigureAwait(false);
                var entityVersion = deliveryDetails.Headers[CustomHttpHeaderKeys.EntityVersion].Single();

                // TODO: Handle concurrency
                await deliveryApiClient.CancelDeliveryAsync(transactionId, entityVersion, cancellationToken).ConfigureAwait(false);

                transactionDocument.DeliveryCreationStepDetails.StepStatus = StepStatus.RolledBack;

                // TODO: Handle concurrency and other errors
                await session.StoreAsync(transactionDocument, changeVector, transactionDocumentId, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}