using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;
using SagaDemo.InventoryAPI.Operations.Responses;
using SagaDemo.InventoryAPI.Utilities.Helpers;

namespace SagaDemo.InventoryAPI.Handlers.CommandHandlers
{
    public class CreateProductCommandHandler : ICreateProductCommandHandler
    {
        private readonly IDocumentStore _documentStore;

        public CreateProductCommandHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<CreateProductResponse> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
        {
            // TODO: Validation
            using (var session = _documentStore.OpenAsyncSession())
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