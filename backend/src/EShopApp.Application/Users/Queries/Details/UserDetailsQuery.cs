using EShopApp.Domain.Entities;
using ErrorOr;
using MediatR;

namespace EShopApp.Application.Users.Queries.Details;

public record UserDetailsQuery(int Id) : IRequest<ErrorOr<User>>;