﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Raven.Client.Documents;
using SagaDemo.InventoryAPI.Extensions;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class TakeoutItemsCommandValidator : AbstractValidator<TakeoutItemsCommand>
    {
        private readonly IDocumentStore documentStore;

        public TakeoutItemsCommandValidator(IDocumentStore documentStore, IValidator<TakeoutItemCommand> childItemValidator)
        {
            this.documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));

            RuleForEach(x => x.Items)
                .SetValidator(childItemValidator);
        }

        public override async Task<ValidationResult> ValidateAsync(ValidationContext<TakeoutItemsCommand> context, CancellationToken cancellationToken)
        {
            using (var session = documentStore.OpenAsyncSession())
            {
                var productsLookup = await session.LoadProductsAsync(context.InstanceToValidate.Items.Select(r => r.ProductId), cancellationToken).ConfigureAwait(false);

                context.RootContextData[ValidationContextKeys.Products] = productsLookup;
            }

            return await base.ValidateAsync(context, cancellationToken);
        }
    }
}