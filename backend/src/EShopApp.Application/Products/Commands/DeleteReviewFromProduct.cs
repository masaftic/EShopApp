using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Errors;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Application.Products.Commands;

public record DeleteReviewFromProductCommand(
    int ProductId,
    int ReviewId) : IRequest<ErrorOr<Deleted>>;

public class DeleteReviewFromProductCommandValidator : AbstractValidator<DeleteReviewFromProductCommand>
{
    public DeleteReviewFromProductCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Product ID is required.");

        RuleFor(x => x.ReviewId)
            .NotEmpty()
            .WithMessage("Review ID is required.");
    }
}

public class DeleteReviewFromProductCommandHandler 
    : IRequestHandler<DeleteReviewFromProductCommand, ErrorOr<Deleted>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public DeleteReviewFromProductCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<ErrorOr<Deleted>> Handle(
        DeleteReviewFromProductCommand request, 
        CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        
        var product = await _dbContext.Products
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound;

        var review = product.Reviews
            .FirstOrDefault(r => r.Id == request.ReviewId && r.UserId == userId);

        if (review is null)
            return DomainErrors.Product.ReviewNotFound;

        product.RemoveReview(review.Id);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Deleted;
    }
}
