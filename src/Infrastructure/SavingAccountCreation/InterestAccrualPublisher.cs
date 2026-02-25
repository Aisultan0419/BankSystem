using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.MessageContracts;
using Domain.Models.Accounts;
using Infrastructure.DbContext;
using Infrastructure.MessageBroker;
using Infrastructure.Serialization;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
namespace Infrastructure.SavingAccountCreation
{
    public class InterestAccrualPublisher
    {
        private readonly AppDbContext _db;
        private readonly IPublishEndpoint _bus;
        private readonly ILogger<OutboxPublisher> _logger;
        public InterestAccrualPublisher(AppDbContext db
            , IPublishEndpoint bus
            , ILogger<OutboxPublisher> logger)
        {
            _db = db;
            _bus = bus;
            _logger = logger;
        }
        public async Task<int> PublishAccountsForAccrualInterest(int limit, CancellationToken ct)
        {
            var savingAccounts = await _db
                .SavingsAccounts
                .Where(sa => sa._interestRate!.EffectiveTo > DateOnly.FromDateTime(DateTime.Today))
                .Where(sa => !_db.InterestAccrualHistory
                    .Any(h => h.AccountId == sa.Id && h.AccrualDate == DateOnly.FromDateTime(DateTime.Today)))
                .Take(limit)
                .ToListAsync(ct);
            _logger.LogInformation("Found {Count} saving accounts to publish for accrual", savingAccounts.Count);

            foreach (var sa in savingAccounts)
            {
                try
                {
                    var dto = new AccrueInterestCommand(
                        AccountId: sa.Id
                    );
                    _logger.LogInformation("Publishing AccrueInterestCommand for AccountId={AccountId}", sa.Id);
                    await _bus.Publish(dto);
                    _logger.LogInformation("PublishSucceeded AccountId={AccountId}", sa.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error publishing interest accrual for SavingAccountId: {SavingAccountId}", sa.Id);
                }
            }
            return savingAccounts.Count;

        }
    }
}
