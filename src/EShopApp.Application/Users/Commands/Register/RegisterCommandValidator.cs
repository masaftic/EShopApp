using EShopApp.Application.Common.Options;
using FluentValidation;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(ApplicationIdentityOptions identityOptions)
    {
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(identityOptions.PasswordRequiredLength).WithMessage($"Password must be at least {identityOptions.PasswordRequiredLength} characters.")
            .Matches(identityOptions.PasswordRequireDigit ? "[0-9]" : "").WithMessage("Password must contain at least one digit.")
            .Matches(identityOptions.PasswordRequireLowercase ? "[a-z]" : "").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(identityOptions.PasswordRequireUppercase ? "[A-Z]" : "").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(identityOptions.PasswordRequireNonAlphanumeric ? "[^a-zA-Z0-9]" : "").WithMessage("Password must contain at least one special character.");
    }
}