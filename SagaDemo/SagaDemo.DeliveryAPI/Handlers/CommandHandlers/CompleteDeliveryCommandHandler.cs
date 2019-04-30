using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Extensions;
using SagaDemo.DeliveryAPI.Operations.Commands;
using SagaDemo.DeliveryAPI.Validation.Validators;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public class CompleteDeliveryCommandHandler : ICompleteDeliveryCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IDeliveryCommandValidator<CompleteDeliveryCommand> requestValidator;

        public CompleteDeliveryCommandHandler(IDocumentStore documentStore, IDeliveryCommandValidator<CompleteDeliveryCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(CompleteDeliveryCommand command, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var deliveryDocument = await session.LoadDeliveryAsync(command.TransactionId, cancellationToken).ConfigureAwait(false);
                var changeVector = session.Advanced.GetChangeVectorFor(deliveryDocument);

                if (deliveryDocument.Status == DeliveryStatus.Finished)
                {
                    return;
                }

                requestValidator.ValidateAndThrow(command, deliveryDocument);

                deliveryDocument.Status = DeliveryStatus.Finished;

                // TODO: Handle concurrency exception
                await session.StoreAsync(deliveryDocument, changeVector, deliveryDocument.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}