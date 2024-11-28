using EShopApp.Application.Common.DTOs;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<ErrorOr<AuthenticationResult>>;