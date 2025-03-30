using ErrorOr;
using MediatR;
using EShopApp.Application.Users.DTOs;

namespace EShopApp.Application.Users.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResponse>>;