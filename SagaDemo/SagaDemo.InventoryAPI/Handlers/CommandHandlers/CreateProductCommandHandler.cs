using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class CreateProductCommandHandler : ICreateProductCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IValidator<CreateProductCommand> requestValidator;

        public CreateProductCommandHandler(IDocumentStore documentStore, IValidator<CreateProductCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task<CreateProductResponse> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
        {
            requestValidator.ValidateAndThrow(command);

            using (var session = documentStore.OpenAsyncSession())
            {
                var documentId = Guid.NewGuid().ToString();
                var productDocument = new Product
                {
                    Id = DocumentIdHelper.GetDocumentId<Product>(session, documentId),
                    Name = command.Name,
                    PointsCost = command.PointsCost,
                    ReservationCount = 0,
                    StockCount = 0
                };

                await session.StoreAsync(productDocument, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new CreateProductResponse(documentId, productDocument.Name, productDocument.PointsCost, productDocument.StockCount);
            }
        }
    }
}