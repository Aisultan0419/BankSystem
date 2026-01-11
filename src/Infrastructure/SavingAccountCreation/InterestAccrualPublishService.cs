
using Infrastructure.MessageBroker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Infrastructure.SavingAccountCreation;
using Microsoft.EntityFrameworkCore.Diagnostics;
namespace Infrastructure.InterestAccrualService
{
    public class InterestAccrualPublishService : BackgroundService
    {
        private readonly IServiceProvider _serProv;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1);
        private readonly ILogger<OutboxHostedService> _logger;
        private readonly int BatchSize = 100;
        public InterestAccrualPublishService(IServiceProvider serProv
            , ILogger<OutboxHostedService> logger)
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
                    var publisher = scope.ServiceProvider.GetRequiredService<InterestAccrualPublisher>();
                    while (true)
                    {
                        var published = await publisher.PublishAccountsForAccrualInterest(BatchSize, stoppingToken);
                        if (published == 0)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Interest accrual publish was not successfull");
                }
                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}
