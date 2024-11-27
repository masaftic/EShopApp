using EShopApp.Application.Common.DTOs;
using EShopApp.Domain.ValueObjects;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Commands.Register;

public record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    Address Address,
    string Password) : IRequest<Result<AuthenticationResult>>;

