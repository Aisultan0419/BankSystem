using Application.DTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;

namespace Application.Services.TransactionServices
{
    public class DepositService : IDepositService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        public DepositService(IAppUserRepository appUserRepository
            ,IUserRepository userRepository
            ,IAccountRepository accountRepository
            ,ITransactionRepository transactionRepository)
        {
            _appUserRepository = appUserRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<DepositResponseDTO> DepositAsync(decimal amount, string appUserId, string lastNumbers)
        {
            const decimal daily_limit = 2000000m;
            Guid.TryParse(appUserId, out var appUserGuid);
            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
            {
                return new DepositResponseDTO
                {
                    message = "AppUser was not found"
                };
            }
            if (amount < 0 || daily_limit > 2000000)
            {
                return new DepositResponseDTO
                {
                    message = "Invalid amount"
                };
            }
            var clientId = appUser.Client.Id;
            var card = await _accountRepository.GetRequisitesDTOAsync(clientId, lastNumbers);
            if (card == null)
            {
                return new DepositResponseDTO
                {
                    message = "Card was not found"
                };
            }
            var account = await _accountRepository.GetAccountById(card.AccountId);
            var tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty"); 
            var kazNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
            var kazToday = DateOnly.FromDateTime(kazNow);
            if (account.LastDepositDateKz != kazToday)
            {
                account.DepositedLastDay = 0m;
                account.LastDepositDateKz = kazToday;
            }
            if (account.DepositedLastDay + amount > daily_limit)
            {
                var canDeposit = daily_limit - account.DepositedLastDay;
                return new DepositResponseDTO
                {
                    message = $"Daily limit exceeded. You can deposit maximum {canDeposit} today."
                };
            }
            using (var tx = await _transactionRepository.BeginTransactionAsync())
            {
                try
                {
                    account.Deposit(amount);
                    account.DepositedLastDay += amount;

                    var transaction = new Transaction
                    {
                        Id = Guid.NewGuid(),
                        From = "External",
                        To = account.Iban.ToString(),
                        ClientId = appUser.Client.Id,
                        Amount = amount,
                        CreatedAt = DateTime.UtcNow,
                        Type = "Deposit"
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


            return new DepositResponseDTO
            {
                message = "Balance is succesfully replenished",
                depositedAmount = amount,
                newBalance = account.Balance
            };
            
        }
    }
}
