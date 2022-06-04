using FluentValidation;
using FluentValidation.Results;

namespace Minibank.Core.UniversalValidators
{
    public class IdEntityValidator : AbstractValidator<string>
    {
        public IdEntityValidator()
        {
            RuleFor(id => id).NotEmpty().WithMessage("id не должен быть пустым");
        }

        protected override bool PreValidate(ValidationContext<string> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure("id", "не может быть пустым"));
                return false;
            }
            return base.PreValidate(context, result);
        }
    }
}