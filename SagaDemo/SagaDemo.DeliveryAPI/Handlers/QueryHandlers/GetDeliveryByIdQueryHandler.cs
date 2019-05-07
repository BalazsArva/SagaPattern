using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using SagaDemo.DeliveryAPI.Extensions;
using SagaDemo.DeliveryAPI.Mappers;
using SagaDemo.DeliveryAPI.Operations.Queries;
using SagaDemo.DeliveryAPI.Operations.Results;

namespace SagaDemo.DeliveryAPI.Handlers.QueryHandlers
{
    public class GetDeliveryByIdQueryHandler : IGetDeliveryByIdQueryHandler
    {
        private readonly IDocumentStore documentStore;

        public GetDeliveryByIdQueryHandler(IDocumentStore documentStore)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetDeliveryByIdQueryResult> HandleAsync(GetDeliveryByIdQuery query, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var deliveryDocument = await session.LoadDeliveryAsync(query.TransactionId, cancellationToken).ConfigureAwait(false);

                if (deliveryDocument == null)
                {
                    return null;
                }

                var changeVector = session.Advanced.GetChangeVectorFor(deliveryDocument);
                var delivery = DeliveryMapper.ToServiceContract(deliveryDocument);

                return new GetDeliveryByIdQueryResult(delivery, changeVector);
            }
        }
    }
}