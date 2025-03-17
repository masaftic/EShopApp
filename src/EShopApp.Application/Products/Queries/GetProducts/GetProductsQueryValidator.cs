using FluentValidation;

namespace EShopApp.Application.Products.Queries.GetProducts;

public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.CategoryId)
            .GreaterThan(0).When(x => x.CategoryId.HasValue)
            .WithMessage("CategoryId must be a positive number.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MinPrice.HasValue)
            .WithMessage("MinPrice must be a positive value.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0).When(x => x.MaxPrice.HasValue)
            .WithMessage("MaxPrice must be a positive value.");

        RuleFor(x => new { x.MinPrice, x.MaxPrice })
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("MinPrice cannot be greater than MaxPrice.");
        
        RuleFor(x => x.SortBy)
            .Must(x => string.IsNullOrEmpty(x) || x == "price" || x == "name")
            .WithMessage("SortBy is optional or can be 'price' or 'name'.");
        
        RuleFor(x => x.SortOrder)
            .Must(x => string.IsNullOrEmpty(x) || x == "asc" || x == "desc")
            .WithMessage("SortOrder is optional or can be 'asc' or 'desc'.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("PageNumber must be 1 or greater.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize must be 1 or greater.")
            .LessThanOrEqualTo(100).WithMessage("PageSize cannot exceed 100.");
    }
}