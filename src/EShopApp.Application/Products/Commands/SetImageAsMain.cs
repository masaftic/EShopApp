using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands;

public record SetImageAsMainCommand(int ProductId, int ImageId) : IRequest<ErrorOr<Success>>;


public class SetImageAsMainCommandValidator : AbstractValidator<SetImageAsMainCommand>
{
    public SetImageAsMainCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage("Image ID is required.");
    }
}


public class SetImageAsMainCommandHandler : IRequestHandler<SetImageAsMainCommand, ErrorOr<Success>>
{
    private readonly IApplicationDbContext _dbContext;

    public SetImageAsMainCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Success>> Handle(SetImageAsMainCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;
        
        var productImage = product.Images.Where(i => i.Id == request.ImageId).FirstOrDefault();

        if (productImage is null)
            return DomainErrors.Product.ImageNotFound;
        
        if (productImage.IsMain)
            return Result.Success;
        
        product.SetMainImage(request.ImageId);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


