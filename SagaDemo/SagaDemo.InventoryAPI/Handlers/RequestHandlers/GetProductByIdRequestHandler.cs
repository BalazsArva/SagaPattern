using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.Common.DataAccess.RavenDb.Utilities;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Requests;
using SagaDemo.InventoryAPI.Operations.Responses;

namespace SagaDemo.InventoryAPI.Handlers.RequestHandlers
{
    public class GetProductByIdRequestHandler : IGetProductByIdRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetProductByIdRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetProductByIdResponse> HandleAsync(GetProductByIdRequest request, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var entityId = DocumentIdHelper.GetEntityId<Product>(session, request.ProductId);

                var entity = await session.LoadAsync<Product>(entityId, cancellationToken).ConfigureAwait(false);

                if (entity == null)
                {
                    return null;
                }

                return new GetProductByIdResponse(request.ProductId, entity.Name, entity.PointsCost, entity.StockCount);
            }
        }
    }
}