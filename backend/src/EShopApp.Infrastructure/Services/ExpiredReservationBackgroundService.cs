using EShopApp.Application.Common.Interfaces.Services;
using EShopApp.Domain.Entities;
using EShopApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EShopApp.Infrastructure.Services;

public class ExpiredReservationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ExpiredReservationBackgroundService> _logger;

    public ExpiredReservationBackgroundService(IServiceProvider serviceProvider,
        ILogger<ExpiredReservationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Expired Reservation Background Service is starting");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reservationService = scope.ServiceProvider.GetRequiredService<IReservationService>();

                var expiredReservations = await dbContext
                    .Reservations
                    .Include(r => r.ReservationItems)
                    .Where(r => r.Status == ReservationStatus.Active && r.ExpirationDate <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var expiredReservation in expiredReservations)
                {
                    using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        // Release the reservation
                        await reservationService.ReleaseReservationAsync(expiredReservation, stoppingToken);

                        await dbContext.SaveChangesAsync(stoppingToken);
                        await transaction.CommitAsync(stoppingToken);

                        _logger.LogInformation("Processed reservation {ReservationId}", expiredReservation.Id);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(stoppingToken);
                        _logger.LogError(ex, "Failed to process reservation {ReservationId}", expiredReservation.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking expired reservations");
            }
            
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}