using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Exceptions;
using SagaDemo.OrderAPI.Entitites;
using SagaDemo.OrderAPI.Orchestrators;

namespace SagaDemo.OrderAPI.Services
{
    public class CreateOrderSchedulingService : BackgroundService
    {
        private const int DefaultPollingBatchSize = 50;
        private const int DefaultPollingIntervalSeconds = 5;
        private const int DefaultLockExpiryMinutes = 5;

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
                var foundProcessableItems = await ProcessCommandsAsync(stoppingToken).ConfigureAwait(false);

                if (!foundProcessableItems)
                {
                    await Task.Delay(TimeSpan.FromSeconds(DefaultPollingIntervalSeconds)).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> ProcessCommandsAsync(CancellationToken cancellationToken)
        {
            QueryStatistics queryStatistics;
            List<string> itemsToProcess;

            using (var session = documentStore.OpenAsyncSession())
            {
                var timeoutLimit = DateTime.UtcNow.AddMinutes(-DefaultLockExpiryMinutes);

                itemsToProcess = await session
                    .Query<OrderTransaction>()
                    .Statistics(out queryStatistics)
                    .Where(t =>
                        (t.TransactionStatus == TransactionStatus.NotStarted || t.TransactionStatus == TransactionStatus.InProgress) &&
                        (t.UtcDateTimeLockAcquired == null || t.UtcDateTimeLockAcquired.Value < timeoutLimit))
                    .Take(DefaultPollingBatchSize)
                    .Select(t => t.Id)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            if (itemsToProcess.Count == 0)
            {
                return false;
            }

            for (var i = 0; i < itemsToProcess.Count; ++i)
            {
                await ProcessCommandAsync(itemsToProcess[i], cancellationToken).ConfigureAwait(false);
            }

            return queryStatistics.TotalResults > DefaultPollingBatchSize;
        }

        private async Task ProcessCommandAsync(string transactionId, CancellationToken cancellationToken)
        {
            OrderTransaction transaction;

            try
            {
                using (var session = documentStore.OpenAsyncSession())
                {
                    transaction = await session.LoadAsync<OrderTransaction>(transactionId, cancellationToken).ConfigureAwait(false);

                    var changeVector = session.Advanced.GetChangeVectorFor(transaction);

                    transaction.UtcDateTimeLockAcquired = DateTime.UtcNow;

                    await session.StoreAsync(transaction, changeVector, transactionId, cancellationToken).ConfigureAwait(false);
                }
            }
            catch (ConcurrencyException)
            {
                // Another process already acquired lock on this transaction, skip it.
                return;
            }

            var command = new Operations.Commands.CreateOrderCommand(
                transaction.OrderDetails.UserId,
                new Operations.DataStructures.Order
                {
                    Items = transaction
                        .OrderDetails
                        .Items
                        .Select(i => new Operations.DataStructures.OrderItem
                        {
                            ProductId = i.ProductId,
                            Quantity = i.Quantity
                        })
                        .ToList()
                },
                new Operations.DataStructures.Address
                {
                    City = transaction.OrderDetails.Address.City,
                    Country = transaction.OrderDetails.Address.Country,
                    House = transaction.OrderDetails.Address.House,
                    State = transaction.OrderDetails.Address.State,
                    Street = transaction.OrderDetails.Address.Street,
                    Zip = transaction.OrderDetails.Address.Zip
                });

            await commandOrchestrator.HandleAsync(command, cancellationToken).ConfigureAwait(false);
        }
    }
}