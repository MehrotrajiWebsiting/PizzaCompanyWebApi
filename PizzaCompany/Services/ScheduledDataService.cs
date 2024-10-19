
using Microsoft.EntityFrameworkCore;
using PizzaCompany.Data;

namespace PizzaCompany.Services
{
    public class ScheduledDataService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ScheduledDataService> _logger;
        private readonly TimeSpan _purgeInterval = TimeSpan.FromMinutes(10);

        public ScheduledDataService(IServiceProvider serviceProvider, ILogger<ScheduledDataService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Delete Audit Data after 4 min
                await PerformDataPurgeAsync(stoppingToken);
                // Wait
                await Task.Delay(_purgeInterval, stoppingToken);
            }
        }

        private async Task PerformDataPurgeAsync(CancellationToken stoppingToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<PizzaCompanyDbContext>();
                _logger.LogInformation("Starting data purge...");

                // Example: Remove records older than 10 min
                await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE AuditLogs;");

                await dbContext.SaveChangesAsync(stoppingToken);
                _logger.LogInformation("Data purge completed.");
            }
        }
    }
}
