using EShopApp.Application.Common.Options;
using FluentValidation;

namespace EShopApp.Application.Users.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(ApplicationIdentityOptions identityOptions)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");
        
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(identityOptions.PasswordRequiredLength).WithMessage($"Password must be at least {identityOptions.PasswordRequiredLength} characters.")
            .Matches(identityOptions.PasswordRequireDigit ? "[0-9]" : "").WithMessage("Password must contain at least one digit.")
            .Matches(identityOptions.PasswordRequireLowercase ? "[a-z]" : "").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(identityOptions.PasswordRequireUppercase ? "[A-Z]" : "").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(identityOptions.PasswordRequireNonAlphanumeric ? "[^a-zA-Z0-9]" : "").WithMessage("Password must contain at least one special character.");
    }
}