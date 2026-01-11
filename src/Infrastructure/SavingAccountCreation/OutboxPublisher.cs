using Application.MessageContracts;
using Infrastructure;
using Infrastructure.DbContext;
using Application.Interfaces.Repositories;
using Infrastructure.Serialization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace Infrastructure.MessageBroker
{
    public class OutboxPublisher 
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _bus;
        private readonly IUserRepository _userRep;
        private readonly ILogger<OutboxPublisher> _logger;
        public OutboxPublisher(AppDbContext db
            , IPublishEndpoint bus
            , IUserRepository userRep
            , ILogger<OutboxPublisher> logger)
        {
            _db = db;
            _bus = bus;
            _userRep = userRep;
            _logger = logger;
        }
        public async Task PublishPendingMessageAsync(int limit, CancellationToken ct)
        {
            var pending = await _db.OutBoxes
                .Where(x => x.ProcessedOn == null)
                .OrderBy(x => x.OccurredOn)
                .Take(limit)
                .ToListAsync(ct);


            foreach (var msg in pending)
            {
                if (msg.RetryCount >= 10) 
                {
                    _logger.LogWarning("Outbox message {MessageId} reached max retry count ({RetryCount}). Skipping.", msg.Id, msg.RetryCount);

                    continue;
                }
                try
                {
                    var obj = JsonSerializer.Deserialize<TransactionRequested>(msg.Payload, JsonDefaults.Options);

                    await _bus.Publish(obj!);

                    msg.ProcessedOn = DateTime.UtcNow;  
                    _logger.LogInformation("Successfully published outbox message {MessageId}.", msg.Id);
                }
                catch
                {



                    msg.RetryCount++;
                    _logger.LogWarning("Failed to publish outbox message {MessageId}. Retry count increased to {RetryCount}.", msg.Id, msg.RetryCount);
                }
            }
            await _userRep.SaveChangesAsync();
        }
    }
}
