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
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly CheckLimit _checkLimit;
        public TransferService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,CheckLimit checkLimit
            ,ITransactionProcessor transactionProcessor)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _checkLimit = checkLimit;
            _transactionProcessor = transactionProcessor;
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
            var resultOfCheck = _checkLimit.checkLimit(fromAccount, amount);
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
            await _transactionProcessor.ProcessTransferAsync(appUser, fromAccount, toAccount, amount);

            return new TransferResponseDTO
            {
                Full_name = toAccount.Client.FullName,
                transferredAmount = amount,
                remainingBalance = fromAccount.Balance
            };
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
