using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.MessageBroker
{
    public class OutboxHostedService : BackgroundService
    {
        private readonly IServiceProvider _serProv;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(1);
        private const int BatchSize = 50;
        private readonly ILogger<OutboxHostedService> _logger;
        public OutboxHostedService(IServiceProvider serProv
            ,ILogger<OutboxHostedService> logger)
        {
            _serProv = serProv;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serProv.CreateScope();
                    var publisher = scope.ServiceProvider.GetRequiredService<OutboxPublisher>();    
                    await publisher.PublishPendingMessageAsync(BatchSize, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while publishing outbox messages.");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
