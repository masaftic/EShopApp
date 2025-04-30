using ErrorOr;
using EShopApp.Application.Common.Interfaces.Caching;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EShopApp.Application.Common.Behaviors;

public class QueryCachingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedQuery
    where TResponse : IErrorOr
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> _logger;

    public QueryCachingPipelineBehavior(IDistributedCache cache, ILogger<QueryCachingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Attempt to retrieve the cached response
        var cachedResponse = await _cache.GetStringAsync(request.CacheKey, cancellationToken);
        if (cachedResponse is not null)
        {
            return HandleCacheHit(request.CacheKey, cachedResponse);
        }

        // Log cache miss and proceed to the next handler
        _logger.LogInformation("Cache miss for key: {CacheKey}", request.CacheKey);
        var response = await next();

        if (response.IsError)
        {
            return response;
        }

        await CacheResponseAsync(request, response, cancellationToken);

        return response;
    }

    // reflection magic is required because we don't know the type of TResponse at compile time

    private TResponse HandleCacheHit(string cacheKey, string cachedResponse)
    {
        _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);

        var deserializedWithType = JsonSerializer.Deserialize<Dictionary<string, object>>(cachedResponse);
        if (deserializedWithType is null)
        {
            return default!;
        }

        if (deserializedWithType.TryGetValue("Type", out var typeName) &&
            deserializedWithType.TryGetValue("Value", out var value))
        {
            var resolvedType = Type.GetType(typeName.ToString()!);
            if (resolvedType is not null)
            {
                var deserializedObject = JsonSerializer.Deserialize(value.ToString()!, resolvedType);
                return (dynamic)deserializedObject!;
            }
        }

        _logger.LogWarning("Failed to deserialize cached response for key: {CacheKey}", cacheKey);
        var error = Error.Failure("CacheError", "Failed to deserialize cached response.");
        return (dynamic)error;
    }

    private async Task CacheResponseAsync(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        var value = ((dynamic)response).Value;
        var type = value.GetType();

        var serializedResponse = JsonSerializer.Serialize(new
        {
            Type = type.AssemblyQualifiedName,
            Value = value
        });

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = request.CacheExpiration
        };

        await _cache.SetStringAsync(request.CacheKey, serializedResponse, options, cancellationToken);
        _logger.LogInformation("Response cached for key: {CacheKey}", request.CacheKey);
    }
}