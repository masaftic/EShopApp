using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.ValueObjects;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;

