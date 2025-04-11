using ErrorOr;
using EShopApp.Application.Common.DTOs;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Products.DTOs;
using FluentValidation;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Queries;

public record GetFilteredProductsQuery(
    string? SearchTerm,
    decimal? MinPrice,
    decimal? MaxPrice,
    int? CategoryId,
    string? SortBy = "name",
    string? SortOrder = "asc",
    int PageNumber = 1,
    int PageSize = 10) : IRequest<ErrorOr<PaginatedList<ProductDto>>>;


public class GetFilteredProductsQueryValidator : AbstractValidator<GetFilteredProductsQuery>
{
    public GetFilteredProductsQueryValidator()
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
        
        RuleFor(x => x.SearchTerm)
            .MaximumLength(100).WithMessage("SearchTerm cannot exceed 100 characters.");

    }
}


public class GetFilteredProductsQueryHandler 
    : IRequestHandler<GetFilteredProductsQuery, ErrorOr<PaginatedList<ProductDto>>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetFilteredProductsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<PaginatedList<ProductDto>>> Handle(
        GetFilteredProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Products.AsQueryable();

        if (!string.IsNullOrEmpty(request.SearchTerm))
            query = query.Where(p => p.Name.Contains(request.SearchTerm) || 
                                   p.Description.Contains(request.SearchTerm));

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == request.CategoryId);

        if (request.MinPrice.HasValue)
            query = query.Where(p => p.Price >= request.MinPrice);

        if (request.MaxPrice.HasValue)
            query = query.Where(p => p.Price <= request.MaxPrice);

        query = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "desc" 
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
            "price" => request.SortOrder?.ToLower() == "desc"
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),
            _ => query.OrderBy(p => p.Name)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip(request.PageSize * (request.PageNumber - 1))
            .Take(request.PageSize)
            .ProjectToType<ProductDto>()
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProductDto>(
            products, totalCount, request.PageSize, request.PageNumber);
    }
}
