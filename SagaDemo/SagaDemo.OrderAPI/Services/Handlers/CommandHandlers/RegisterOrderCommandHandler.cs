using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.OrderAPI.Entitites;
using SagaDemo.OrderAPI.Mappers;
using SagaDemo.OrderAPI.Operations.Commands;
using SagaDemo.OrderAPI.Providers;

namespace SagaDemo.OrderAPI.Services.Handlers.CommandHandlers
{
    public class RegisterOrderCommandHandler : IRegisterOrderCommandHandler
    {
        private readonly IGuidProvider guidProvider;
        private readonly IDocumentStore documentStore;

        public RegisterOrderCommandHandler(IGuidProvider guidProvider, IDocumentStore documentStore)
        {
            this.guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<string> HandleAsync(RegisterOrderCommand command, CancellationToken cancellationToken)
        {
            var transactionId = guidProvider.GenerateGuidString();

            using (var session = documentStore.OpenAsyncSession())
            {
                var transactionDocumentId = DocumentIdHelper.GetDocumentId<OrderTransaction>(documentStore, transactionId);
                var transactionDocument = new OrderTransaction
                {
                    Id = transactionDocumentId,
                    TransactionStatus = TransactionStatus.NotStarted,
                    LoyaltyPointsConsumptionStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        RollbackAttempts = 0,
                        StepStatus = StepStatus.NotStarted
                    },
                    DeliveryCreationStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        RollbackAttempts = 0,
                        StepStatus = StepStatus.NotStarted
                    },
                    InventoryReservationStepDetails = new StepDetails
                    {
                        Attempts = 0,
                        RollbackAttempts = 0,
                        StepStatus = StepStatus.NotStarted
                    },
                    OrderTotalStepDetails = new OrderTotalStepDetails
                    {
                        Attempts = 0,
                        RollbackAttempts = 0,
                        StepStatus = StepStatus.NotStarted,
                        Total = 0
                    },
                    OrderDetails = new OrderDetails
                    {
                        UserId = command.UserId,
                        Address = AddressMapper.ToEntity(command.Address),
                        Items = OrderMapper.ToEntities(command.Order)
                    }
                };

                await session.StoreAsync(transactionDocument, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync().ConfigureAwait(false);
            }

            return transactionId;
        }
    }
}