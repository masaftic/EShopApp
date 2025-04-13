using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Application.Products.DTOs;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using FluentValidation;
using Mapster;
using MediatR;

namespace EShopApp.Application.Products.Commands;

public record AddImageToProductCommand(int ProductId, string FileName, string ContentType, byte[] BinaryContent) : IRequest<ErrorOr<Created>>;


public class AddImageToProductCommandValidator : AbstractValidator<AddImageToProductCommand>
{
    private readonly List<string> allowedImageTypes = ["image/jpeg", "image/png", "image/gif", "image/webp", "image/bmp", "image/tiff"];

    public AddImageToProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.FileName)
            .NotEmpty()
            .WithMessage("File name is required.")
            .Matches(@"^[^\\/:*?""<>|]+$")
            .WithMessage("File name contains invalid characters.");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .WithMessage("Content type is required.")
            .Must(contentType => allowedImageTypes.Contains(contentType))
            .WithMessage("Invalid content type. Only JPEG, PNG, GIF, WEBP, BMP, and TIFF images are allowed.");

        RuleFor(x => x.BinaryContent)
            .NotEmpty()
            .WithMessage("Binary content is required.");
    }
}


public class AddImageToProductCommandHandler : IRequestHandler<AddImageToProductCommand, ErrorOr<Created>>
{
    private readonly IImageStorageService _imageStorageService;
    private readonly IApplicationDbContext _dbContext;

    public AddImageToProductCommandHandler(IImageStorageService imageStorageService, IApplicationDbContext dbContext)
    {
        _imageStorageService = imageStorageService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Created>> Handle(AddImageToProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products.FindAsync([request.ProductId], cancellationToken);
        if (product is null)
            return DomainErrors.Product.NotFound;

        if (product.Images.Count >= 5)
            return DomainErrors.Product.TooManyImages;

        var uniqueImageFileName = $"{Guid.NewGuid()}{Path.GetExtension(request.FileName)}";

        var uploadResponse = await _imageStorageService.SaveAsync(uniqueImageFileName, request.ContentType, request.BinaryContent, cancellationToken);
        if (uploadResponse.IsError)
            return uploadResponse.Errors;

        var productImage = new ProductImage(product.Id, uploadResponse.Value, request.FileName);
        product.AddImage(productImage);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Created;
    }
}


