using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Application.Services.TransactionServices
{
    public class TransactionProcessor : ITransactionProcessor
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
        private DateTime KazNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        private DateOnly KazToday => DateOnly.FromDateTime(KazNow);
        public TransactionProcessor(
            ITransactionRepository transactionRepository,
            IUserRepository userRepository)
        {
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
        }

        public async Task ProcessTransferAsync(AppUser appUser, Account fromAccount, Account toAccount, decimal amount)
        {
            var strategy = _transactionRepository.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await using (var tx = await _transactionRepository.BeginTransactionAsync())
                {
                    try
                    {
                        var money = new Money(amount, fromAccount.Currency ?? "KZT");
                        toAccount.Deposit(amount);
                        fromAccount.TransferOut(amount);
                        fromAccount.TransferredLastDayMoney = (fromAccount.TransferredLastDayMoney ?? new Money(0m, money.Currency)) + money;
                        fromAccount.LastTransferDateKz = KazToday;

                        var transaction = new Transaction
                        {
                            Id = Guid.NewGuid(),
                            From = fromAccount.Iban.ToString(),
                            To = toAccount.Iban.ToString(),
                            ClientId = appUser!.Client.Id,
                            AmountMoney = money,
                            CreatedAt = DateTime.UtcNow,
                            Type = "Transfer"
                        };

                        await _transactionRepository.AddTransaction(transaction);
                        await _userRepository.SaveChangesAsync();

                        await tx.CommitAsync();
                    }
                    catch
                    {
                        await tx.RollbackAsync();
                        throw;
                    }
                }
            });
        }
    }
}
