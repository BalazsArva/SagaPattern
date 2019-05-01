using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.InventoryAPI.Operations.Commands;

namespace SagaDemo.InventoryAPI.Validation.Validators
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.PointsCost)
                .GreaterThan(0)
                .WithMessage(CommonValidationMessages.MustBeGreaterThanZero);
        }
    }
}