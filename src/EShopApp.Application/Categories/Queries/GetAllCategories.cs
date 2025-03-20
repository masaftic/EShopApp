using ErrorOr;
using EShopApp.Application.Categories.DTOs;
using EShopApp.Application.Common.Helpers;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Categories.Queries;

public record GetAllCategoriesQuery : IRequest<ErrorOr<List<CategoryDto>>>;


public class GetCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ErrorOr<List<CategoryDto>>>
{
    private readonly IApplicationDbContext _dbContext;
    public GetCategoriesQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async Task<ErrorOr<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _dbContext
            .Categories
            .ProjectToType<CategoryDto>()
            .ToListAsync(cancellationToken: cancellationToken); 
    }
}