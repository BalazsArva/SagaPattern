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
    public class RegisterDeliveryAttemptCommandHandler : IRegisterDeliveryAttemptCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<RegisterDeliveryAttemptCommand> requestValidator;

        public RegisterDeliveryAttemptCommandHandler(IDocumentStore documentStore, IValidator<RegisterDeliveryAttemptCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(RegisterDeliveryAttemptCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                var deliveryDocument = await session.LoadDeliveryAsync(command.TransactionId, cancellationToken).ConfigureAwait(false);

                deliveryDocument.Status = DeliveryStatus.InProgress;

                await session.StoreAsync(deliveryDocument, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}