using Application.Interfaces;
using Application.MessageContracts;
using Domain.Models;
using Domain.Models.Accounts;
using Infrastructure.DbContext;
using Infrastructure.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
namespace Infrastructure.SavingAccountCreation
{
    public sealed class OutboxWriter : IOutboxWriter
    {
        private readonly AppDbContext _db;
        private readonly IClock _clock;
        public OutboxWriter(AppDbContext db, IClock clock)
        {
            _db = db;
            _clock = clock;
        }
        public async Task Add<T>(T message) where T : IOutboxMessage
        {
            if (await _db.OutBoxes.AnyAsync(x => x.Id == message.MessageId))
                return;
            var payload = JsonSerializer.Serialize(message, JsonDefaults.Options);

            await _db.OutBoxes.AddAsync(new Outbox
            {
                Id = message.MessageId,
                EventType = typeof(T).Name,
                Payload = payload,
                OccurredOn = _clock.UtcNow,
                ProcessedOn = null,
                RetryCount = 0
            });
        }

    }

}
