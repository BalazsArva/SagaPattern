﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Exceptions;
using SagaDemo.Common.Errors;
using SagaDemo.DeliveryAPI.Entities;
using SagaDemo.DeliveryAPI.Extensions;
using SagaDemo.DeliveryAPI.Operations.Commands;
using SagaDemo.DeliveryAPI.Validation.Validators;

namespace SagaDemo.DeliveryAPI.Handlers.CommandHandlers
{
    public class CancelDeliveryCommandHandler : ICancelDeliveryCommandHandler
    {
        private readonly IDocumentStore documentStore;
        private readonly IDeliveryCommandValidator<CancelDeliveryCommand> requestValidator;

        public CancelDeliveryCommandHandler(IDocumentStore documentStore, IDeliveryCommandValidator<CancelDeliveryCommand> requestValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            this.requestValidator = requestValidator ?? throw new ArgumentNullException(nameof(requestValidator));
        }

        public async Task HandleAsync(CancelDeliveryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                using (var session = documentStore.OpenAsyncSession())
                {
                    var deliveryDocument = await session.LoadDeliveryAsync(command.TransactionId, cancellationToken).ConfigureAwait(false);

                    requestValidator.ValidateAndThrow(command, deliveryDocument);

                    if (deliveryDocument.Status == DeliveryStatus.Cancelled)
                    {
                        return;
                    }

                    deliveryDocument.Status = DeliveryStatus.Cancelled;

                    await session.StoreAsync(deliveryDocument, command.DocumentVersion, deliveryDocument.Id, cancellationToken).ConfigureAwait(false);
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (ConcurrencyException ce)
            {
                throw new ConcurrentUpdateException("The entity requested for modification has been modified by another client since it was loaded.", ce);
            }
        }
    }
}