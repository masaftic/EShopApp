using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands;

public record RemoveImageFromProductCommand(int ProductId, int ImageId) : IRequest<ErrorOr<Deleted>>;


public class RemoveImageFromProductCommandValidator : AbstractValidator<RemoveImageFromProductCommand>
{
    public RemoveImageFromProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage("Image ID is required.");
    }
}


public class RemoveImageFromProductCommandHandler : IRequestHandler<RemoveImageFromProductCommand, ErrorOr<Deleted>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IApplicationDbContext _dbContext;

    public RemoveImageFromProductCommandHandler(IImageStorageService imageStorageService, IApplicationDbContext dbContext)
    {
        _imageStorageService = imageStorageService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Deleted>> Handle(RemoveImageFromProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;
        
        var productImage = product.Images.Where(i => i.Id == request.ImageId).FirstOrDefault();

        if (productImage is null)
            return DomainErrors.Product.ImageNotFound;
        
        product.RemoveImage(request.ImageId);
        
        var result = await _imageStorageService.DeleteAsync(productImage.ImageKey);
        if (result.IsError)
            return result.Errors;
            
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}


