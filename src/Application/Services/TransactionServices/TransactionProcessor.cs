using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            using (var tx = await _transactionRepository.BeginTransactionAsync())
            {
                try
                {
                    toAccount.Deposit(amount);
                    fromAccount.TransferOut(amount);
                    fromAccount.TransferredLastDay += amount;
                    fromAccount.LastTransferDateKz = KazToday;

                    var transaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        From = fromAccount.Iban.ToString(),
                        To = toAccount.Iban.ToString(),
                        ClientId = appUser!.Client.Id,
                        Amount = amount,
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
        }
    }
}
