using FluentValidation;

namespace Minibank.Core.Domains.Users.Validators
{
    public class CreateUserValidator : AbstractValidator<User>
    {
        public CreateUserValidator()
        {
            RuleFor(user => user.Login).NotEmpty().WithMessage("не должен быть пустым");
            RuleFor(user => user.Email).EmailAddress().When(user => !string.IsNullOrEmpty(user.Email))
                .WithMessage("адрес электронной почты имеет неверную форму");
        }
    }
}