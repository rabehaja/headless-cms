using Entries.Api.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Entries.Api.Background;

public class EntryScheduler : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EntryScheduler> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(30);

    public EntryScheduler(IServiceProvider serviceProvider, ILogger<EntryScheduler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var scheduler = scope.ServiceProvider.GetRequiredService<EntrySchedulingService>();
                await scheduler.ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process entry schedules");
            }

            await Task.Delay(_interval, stoppingToken);
        }
    }
}
