using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Raven.Client.Documents;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Extensions;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public class CancelDeliveryCommandHandler : ICancelDeliveryCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<CancelDeliveryCommand> requestValidator;

        public CancelDeliveryCommandHandler(IDocumentStore documentStore, IValidator<CancelDeliveryCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(CancelDeliveryCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                // TODO: Validate state and existence
                var deliveryDocument = await session.LoadDeliveryAsync(command.TransactionId, cancellationToken).ConfigureAwait(false);
                var changeVector = session.Advanced.GetChangeVectorFor(deliveryDocument);

                deliveryDocument.Status = DeliveryStatus.Cancelled;

                // TODO: Handle concurrency exception
                await session.StoreAsync(deliveryDocument, changeVector, deliveryDocument.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}