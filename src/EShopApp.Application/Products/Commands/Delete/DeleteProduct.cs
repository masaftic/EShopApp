using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using MediatR;

namespace EShopApp.Application.Products.Commands.Delete;


public record DeleteProductCommand(int Id) : IRequest<ErrorOr<Deleted>>;


public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;

    public DeleteProductHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync(request.Id, cancellationToken);
        if (product == null)
            return Errors.Product.NotFound;
        
        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
