using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands;

public record AddReviewToProductCommand(
    int ProductId,
    string Comment,
    int Rating) : IRequest<ErrorOr<Created>>;

public class AddReviewToProductCommandValidator : AbstractValidator<AddReviewToProductCommand>
{
    public AddReviewToProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Comment cannot be empty and must not exceed 500 characters.");
    }
}

public class AddReviewToProductCommandHandler 
    : IRequestHandler<AddReviewToProductCommand, ErrorOr<Created>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public AddReviewToProductCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Created>> Handle(
        AddReviewToProductCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        
        var product = await _dbContext.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;

        if (product.Reviews.Any(r => r.UserId == userId))
            return DomainErrors.Product.ReviewAlreadyExists;

        var review = new ProductReview(request.ProductId, userId, request.Comment, request.Rating);

        product.AddReview(review);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Created;
    }
}
