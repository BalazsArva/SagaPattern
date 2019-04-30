using FluentValidation;
using SagaDemo.Common.Validation;
using SagaDemo.DeliveryAPI.Operations.DataStructures;

namespace SagaDemo.DeliveryAPI.Validation.Validators
{
    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(x => x.Country)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.Zip)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);

            RuleFor(x => x.House)
                .NotEmpty()
                .WithMessage(CommonValidationMessages.CannotBeNullOrEmpty);
        }
    }
}