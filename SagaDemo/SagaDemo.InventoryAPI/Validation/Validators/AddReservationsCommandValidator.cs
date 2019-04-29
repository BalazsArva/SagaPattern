﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Entities;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class AddReservationsCommandValidator : AbstractValidator<AddReservationsCommand>
    {
        private readonly IDocumentStore documentStore;

        public AddReservationsCommandValidator(IDocumentStore documentStore, IValidator<AddReservationCommand> childItemValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));

            RuleForEach(x => x.Reservations)
                .SetValidator(childItemValidator);
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<AddReservationsCommand> context, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var productIds = context.InstanceToValidate.Reservations.Select(r => r.ProductId).ToList();
                var productsLookup = await session.LoadAsync<Product>(productIds, cancellationToken).ConfigureAwait(false);

                context.RootContextData[ValidationContextKeys.Products] = productsLookup;
            }

            return await base.ValidateAsync(context, cancellationToken);
        }
    }
}