using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Operations.Queries;
using SagaDemo.DeliveryAPI.Operations.Results;

namespace SagaDemo.DeliveryAPI.Handlers.QueryHandlers
{
    public class GetDeliveryByIdQueryHandler : IGetDeliveryByIdQueryHandler
    {
        private readonly IDocumentStore documentStore;

        public GetDeliveryByIdQueryHandler(IDocumentStore documentStore)
        {
            this.documentStore = documentStore ?? throw new System.ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetDeliveryByIdQueryResult> HandleAsync(GetDeliveryByIdQuery query, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var deliveryDocument = await session.LoadAsync<Delivery>(query.TransactionId, cancellationToken).ConfigureAwait(false);

                // TODO: Finish implementation, include document version
                return new GetDeliveryByIdQueryResult();
            }
        }
    }
}