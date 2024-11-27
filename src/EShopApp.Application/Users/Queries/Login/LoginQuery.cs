using EShopApp.Application.Common.DTOs;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Queries.Login;

public record LoginQuery(
    string Email,
    string Password) : IRequest<Result<AuthenticationResult>>;