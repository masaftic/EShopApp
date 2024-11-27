using EShopApp.Domain.Entities;
using FluentResults;
using MediatR;

namespace EShopApp.Application.Users.Queries.Details;

public record UserDetailsQuery(Guid Id) : IRequest<Result<User>>;