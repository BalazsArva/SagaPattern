using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;
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

        public async Task<int> HandleAsync(CreateProductCommand command, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var productDocument = new Product
                {
                    Name = command.Name,
                    RequestCount = 0,
                    StockCount = 0
                };

                await session.StoreAsync(productDocument, cancellationToken).ConfigureAwait(false);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return DocumentIdHelper.GetEntityId<Product>(session, productDocument.Id);
            }
        }
    }
}