using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Models;
using System.Runtime.CompilerServices;
namespace Application.Services.TransactionServices
{
    public class TransferService : ITransferService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransactionRepository _transactionRepository;
        private const decimal daily_limit = 2000000m;
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
        private DateTime KazNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        private DateOnly KazToday => DateOnly.FromDateTime(KazNow);
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
            var lookup = await findAccount(appUserId, lastNumbers);
            if (!lookup.Success)
                return new TransferResponseDTO { message = lookup.ErrorMessage };

            var fromAccount = lookup.Account!;
            var appUser = lookup.AppUser!;
            if (fromAccount.Balance < amount)
            {
                return new TransferResponseDTO
                {
                    message = "Insufficient funds"
                };
            }
            var resultOfCheck = checkLimit(fromAccount, amount);
            if (resultOfCheck.Item1 == false)
            {
                return new TransferResponseDTO
                {
                    message = $"You can transfer only {resultOfCheck.Item2} for today"
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

            return new TransferResponseDTO
            {
                Full_name = toAccount.Client.FullName,
                transferredAmount = amount,
                remainingBalance = fromAccount.Balance
            };

        }
        private (bool, decimal?) checkLimit(Account account, decimal amount)
        {
            if (account.LastTransferDateKz != KazToday)
            {
                account.TransferredLastDay = 0m;
                account.LastTransferDateKz = KazToday;
            }
            if (account.TransferredLastDay + amount > daily_limit)
            {
                var canDeposit = daily_limit - account.TransferredLastDay;
                return (false, canDeposit);
            }
            return (true, null);
        }
        private async Task<AccountLookupResult> findAccount(string appUserId, string lastNumbers)
        {
            if (!Guid.TryParse(appUserId, out var appUserGuid))
                return AccountLookupResult.Fail("Invalid user id format");

            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
                return AccountLookupResult.Fail("AppUser was not found");

            var card = await _accountRepository.GetRequisitesDTOAsync(appUser.Client.Id, lastNumbers);
            if (card == null)
                return AccountLookupResult.Fail("Card was not found");

            var account = await _accountRepository.GetAccountById(card.AccountId);
            if (account == null)
                return AccountLookupResult.Fail("Account was not found");

            return AccountLookupResult.Ok(account, appUser);
        }
        private class AccountLookupResult
        {
            public bool Success { get; private set; }
            public string? ErrorMessage { get; private set; }
            public Account? Account { get; private set; }
            public AppUser? AppUser { get; private set; }

            private AccountLookupResult() { }

            public static AccountLookupResult Fail(string message) =>
                new AccountLookupResult { Success = false, ErrorMessage = message };

            public static AccountLookupResult Ok(Account account, AppUser appUser) =>
                new AccountLookupResult { Success = true, Account = account, AppUser = appUser };
        }
    }
}
