using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands;

public record UpdateProductReviewCommand(
    int ProductId,
    int ReviewId,
    string Comment,
    int Rating) : IRequest<ErrorOr<Updated>>;

public class UpdateProductReviewCommandValidator : AbstractValidator<UpdateProductReviewCommand>
{
    public UpdateProductReviewCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.ReviewId)
            .NotEmpty()
            .WithMessage("Review ID is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty()
            .MaximumLength(500)
            .WithMessage("Comment cannot be empty and must not exceed 500 characters.");
    }
}

public class UpdateProductReviewCommandHandler 
    : IRequestHandler<UpdateProductReviewCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public UpdateProductReviewCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Updated>> Handle(
        UpdateProductReviewCommand request, 
        CancellationToken cancellationToken)
    {
        var product = await _dbContext.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;
        
        int userId = int.Parse(_currentUserService.UserId);

        var review = product.Reviews
            .FirstOrDefault(r => r.Id == request.ReviewId && r.UserId == userId);

        if (review is null)
            return DomainErrors.Product.ReviewNotFound;
        
        review.UpdateReview(request.Comment, request.Rating);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}
