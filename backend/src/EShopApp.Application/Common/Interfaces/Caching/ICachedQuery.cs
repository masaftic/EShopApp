using MediatR;

namespace EShopApp.Application.Common.Interfaces.Caching;

public interface ICachedQuery<TResponse> : IRequest<TResponse>, ICachedQuery;

public interface ICachedQuery
{
    string CacheKey { get; }
    TimeSpan CacheExpiration { get; }
}
