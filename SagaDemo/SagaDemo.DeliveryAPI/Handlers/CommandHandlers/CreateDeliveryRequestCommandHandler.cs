using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Commands;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public class CreateDeliveryRequestCommandHandler : ICreateDeliveryRequestCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<CreateDeliveryRequestCommand> requestValidator;

        public CreateDeliveryRequestCommandHandler(IDocumentStore documentStore, IValidator<CreateDeliveryRequestCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(CreateDeliveryRequestCommand command, CancellationToken cancellationToken)
        {
            await requestValidator.ValidateAndThrowAsync(command, cancellationToken: cancellationToken).ConfigureAwait(false);

            using (var session = documentStore.OpenAsyncSession())
            {
                var deliveryDocument = new Delivery
                {
                    Id = DocumentIdHelper.GetDocumentId<Delivery>(session, command.TransactionId),
                    Status = DeliveryStatus.Created,
                    Address = new Address
                    {
                        Country = command.Address.Country,
                        City = command.Address.City,
                        House = command.Address.House,
                        State = command.Address.State,
                        Street = command.Address.Street,
                        Zip = command.Address.Zip
                    }
                };

                await session.StoreAsync(deliveryDocument, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}