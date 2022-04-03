using FluentValidation;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(user => user.Login).NotEmpty().WithMessage("не должен быть пустым");
            RuleFor(user => user.Login.Length).GreaterThanOrEqualTo(8).WithMessage(
                "длина должна быть более 7 символов");
            RuleFor(user => user.Login.Length).LessThanOrEqualTo(24).WithMessage(
                "длина должна быть менее или ровна 24 символов");
            RuleFor(user => user.Email).EmailAddress().When(user => !string.IsNullOrEmpty(user.Email))
                .WithMessage("адрес электронной почты имеет неверную форму");
        }
    }
}