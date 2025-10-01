using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
namespace Application.Services.TransactionServices
{
    public class TransferService : ITransferService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransferService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,IUserRepository userRepository
            ,ITransactionRepository transactionRepository)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<TransferResponseDTO> TransferAsync(string appUserId, string iban, decimal amount, string lastNumbers)
        {
            const decimal daily_limit = 2000000m;
            Guid.TryParse(appUserId, out var appUserGuid);
            if (amount < 0 || daily_limit < amount)
            {
                return new TransferResponseDTO
                {
                    message = "Invalid amount"
                };
            }
            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
            {
                return new TransferResponseDTO
                {
                    message = "AppUser was not found"
                };
            }

            var clientId = appUser.Client.Id;
            var card = await _accountRepository.GetRequisitesDTOAsync(clientId, lastNumbers);
            if (card == null)
            {
                return new TransferResponseDTO
                {
                    message = "Card was not found"
                };
            }
            var fromAccount = await _accountRepository.GetAccountById(card.AccountId);
            if (fromAccount.Balance < amount)
            {
                return new TransferResponseDTO
                {
                    message = "Insufficient funds"
                };
            }
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
            var kazNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var kazToday = DateOnly.FromDateTime(kazNow);
            if (fromAccount.LastTransferDateKz != kazToday)
            {
                fromAccount.TransferredLastDay = 0m;
                fromAccount.LastTransferDateKz = kazToday;
            }
            if (fromAccount.TransferredLastDay + amount > daily_limit)
            {
                var canDeposit = daily_limit - fromAccount.TransferredLastDay;
                return new TransferResponseDTO
                {
                    message = $"Daily limit exceeded. You can deposit maximum {canDeposit} today."
                };
            }
            var toAccount = await _accountRepository.GetAccountByIban(iban);
            if (toAccount == null)
            {
                return new TransferResponseDTO
                {
                    message = "Recipient account was not found"
                };
            }
            using (var tx = await _transactionRepository.BeginTransactionAsync())
            {
                try
                {
                    toAccount.Deposit(amount);
                    fromAccount.TransferOut(amount);
                    fromAccount.TransferredLastDay += amount;
                    fromAccount.LastTransferDateKz = kazToday;
                    var transaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        From = fromAccount.Iban.ToString(),
                        To = toAccount.Iban.ToString(),
                        ClientId = appUser.Client.Id,
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

            return new TransferResponseDTO
            {
                Full_name = toAccount.Client.FullName,
                transferredAmount = amount,
                remainingBalance = fromAccount.Balance
            };

        }
    }
}
