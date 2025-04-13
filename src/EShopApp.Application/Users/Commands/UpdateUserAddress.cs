using ErrorOr;
using EShopApp.Application.Common.Interfaces.Persistence;
using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.ValueObjects;
using FluentValidation;
using MediatR;

namespace EShopApp.Application.Users.Commands;

public record UpdateUserAddressCommand(
    string AddressLine1,
    string AddressLine2,
    string City,
    string State,
    string ZipCode) : IRequest<ErrorOr<Updated>>;


public class UpdateUserAddressCommandValidator : AbstractValidator<UpdateUserAddressCommand>
{
    public UpdateUserAddressCommandValidator()
    {
        RuleFor(x => x.AddressLine1)
            .NotEmpty().WithMessage("Address Line 1 is required.").
            MaximumLength(100).WithMessage("Address Line 1 must be at most 100 characters long.");

        RuleFor(x => x.AddressLine2)
            .NotEmpty().WithMessage("Address Line 2 is required.")
            .MaximumLength(100).WithMessage("Address Line 2 must be at most 100 characters long.");

        RuleFor(x => x.ZipCode)
            .NotEmpty().WithMessage("Zip Code is required.")
            .MaximumLength(10).WithMessage("Zip Code must be at most 10 characters long.")
            .Matches(@"^\d{5}(-\d{4})?$").WithMessage("Zip Code must be in the format 12345 or 12345-6789.");

        RuleFor(x => x.City)
            .NotEmpty().WithMessage("City is required.")
            .MaximumLength(50).WithMessage("City must be at most 50 characters long.");

        RuleFor(x => x.State)
            .NotEmpty().WithMessage("State is required.")
            .MaximumLength(50).WithMessage("State must be at most 50 characters long.");
    }
}


public class UpdateUserAddressCommandHandler : IRequestHandler<UpdateUserAddressCommand, ErrorOr<Updated>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;

    public UpdateUserAddressCommandHandler(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }

    public async Task<ErrorOr<Updated>> Handle(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(_currentUserService.UserId);
        var user = await _dbContext.DomainUsers.FindAsync([userId], cancellationToken);
        if (user is null)
        {
            return Error.Unexpected(
                code: "User.Null",
                description: "Unexpected User is null. This should never happen.");
        }

        var address = new Address(request.AddressLine1, request.AddressLine2, request.City, request.State, request.ZipCode);

        user.UpdateAddress(address);
        _dbContext.DomainUsers.Update(user);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Updated;
    }
}