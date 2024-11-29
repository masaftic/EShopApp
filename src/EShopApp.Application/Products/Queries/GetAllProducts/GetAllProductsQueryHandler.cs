using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Entities;
using MediatR;

namespace EShopApp.Application.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ErrorOr<List<Product>>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }


    public async Task<ErrorOr<List<Product>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return (await _productRepository.GetAllAsync()).ToList();
    }
}