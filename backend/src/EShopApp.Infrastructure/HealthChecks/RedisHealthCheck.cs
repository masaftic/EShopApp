using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace EShopApp.Infrastructure.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisHealthCheck(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _connectionMultiplexer.IsConnected
                ? HealthCheckResult.Healthy("Redis is healthy")
                : HealthCheckResult.Unhealthy("Redis is unhealthy"));
    }
}