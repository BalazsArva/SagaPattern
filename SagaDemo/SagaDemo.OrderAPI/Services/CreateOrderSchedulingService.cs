using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using SagaDemo.OrderAPI.Entitites;
using SagaDemo.OrderAPI.Orchestrators;

namespace SagaDemo.OrderAPI.Services
{
    public class CreateOrderSchedulingService : BackgroundService
    {
        private const int DefaultBatchSize = 10;
        private const int PollingIntervalMilliseconds = 1000;

        private readonly IDocumentStore documentStore;
        private readonly ICreateOrderCommandOrchestrator commandOrchestrator;

        public CreateOrderSchedulingService(IDocumentStore documentStore, ICreateOrderCommandOrchestrator commandOrchestrator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.commandOrchestrator = commandOrchestrator ?? throw new ArgumentNullException(nameof(commandOrchestrator));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ScheduleCommandsAsync(stoppingToken).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromMilliseconds(PollingIntervalMilliseconds)).ConfigureAwait(false);
            }
        }

        private async Task ScheduleCommandsAsync(CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                // TODO: Include other possible statuses as well, but consider possible parallel processing issues (e.g. introduce reservation, etc.)
                var itemsToProcess = await session
                    .Query<OrderTransaction>()
                    .Where(t => t.TransactionStatus == TransactionStatus.NotStarted)
                    .Take(DefaultBatchSize)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }
}