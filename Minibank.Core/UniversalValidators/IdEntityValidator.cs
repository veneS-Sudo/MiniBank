using FluentValidation;

namespace Minibank.Core.UniversalValidators
{
    public class IdEntityValidator : AbstractValidator<string>
    {
        public IdEntityValidator()
        {
            RuleFor(id => id).NotEmpty().WithMessage("id не должен быть пустым");
        }
    }
}