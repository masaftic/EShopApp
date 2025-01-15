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
    }

    [GeneratedRegex(@"^(/(\d+))+$")]
    private static partial Regex CategoryPathPattern(); // ex. /1/4/6
}
