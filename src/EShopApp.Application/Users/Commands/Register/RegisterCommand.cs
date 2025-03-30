using EShopApp.Domain.ValueObjects;
using ErrorOr;
using MediatR;
using EShopApp.Application.Users.DTOs;

namespace EShopApp.Application.Users.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResponse>>;

