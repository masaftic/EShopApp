using System;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using FluentValidation;

namespace EShopApp.Application.Categories.Commands.Add;

public partial class AddCategoryCommandValidator : AbstractValidator<AddCategoryCommand>
{
    public AddCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Path)
            .NotEmpty().WithMessage("Path is required")
            .MaximumLength(100).WithMessage("Path must not exceed 100 characters");

        RuleFor(x => x.Path)
            .Must(x => x == "" || CategoryPathPattern().Match(x).Success)
            .WithMessage("Path must be a valid path");
    }

    [GeneratedRegex(@"^(/(\d+))+$")]
    private static partial Regex CategoryPathPattern(); // ex. /1/4/6
}
