using EShopApp.Application.Common.Interfaces.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EShopApp.Infrastructure.Services;

class ExpiredRefreshTokensBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiredRefreshTokensBackgroundService> _logger;

    public ExpiredRefreshTokensBackgroundService(IServiceProvider serviceProvider, ILogger<ExpiredRefreshTokensBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ExpiredRefreshTokensBackgroundService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            var expiredTokens = dbContext.RefreshTokens
                .Where(r => r.ExpiresOnUtc < DateTime.UtcNow);

            dbContext.RefreshTokens.RemoveRange(expiredTokens);
            await dbContext.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        _logger.LogInformation("ExpiredRefreshTokensBackgroundService stopped.");
    }
}