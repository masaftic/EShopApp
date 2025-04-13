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

                var expiredReservations = await dbContext
                    .Reservations
                    .Include(r => r.ReservationItems)
                    .Where(r => r.Status == ReservationStatus.Active && r.ExpirationDate <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var expiredReservation in expiredReservations)
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync(stoppingToken);

                    try
                    {
                        // Release reserved stock
                        var productIds = expiredReservation.ReservationItems
                            .Select(ri => ri.ProductId)
                            .ToList();

                        var inventories = await dbContext.Inventories
                            .Where(i => productIds.Contains(i.ProductId))
                            .ToListAsync(stoppingToken);

                        foreach (var item in expiredReservation.ReservationItems)
                        {
                            var inventory = inventories.First(i => i.ProductId == item.ProductId);
                            inventory.Release(item.Quantity); // Releases reserved stock

                            // Log inventory transaction
                            dbContext.InventoryTransactions.Add(new InventoryTransaction(
                                inventory.Id,
                                item.Quantity,
                                InventoryTransactionType.Release,
                                DateTime.UtcNow,
                                $"Expired Reservation: {expiredReservation.Id}"
                            ));
                        }
                        
                        expiredReservation.ReservationItems.Clear();

                        // Update reservation status
                        expiredReservation.Status = ReservationStatus.Expired;
                        expiredReservation.UpdatedAt = DateTime.UtcNow;

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