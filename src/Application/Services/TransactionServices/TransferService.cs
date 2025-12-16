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
        private readonly IFindAccountService _findAccountService;
        public TransferService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,CheckLimit checkLimit
            ,ITransactionProcessor transactionProcessor
            ,IFindAccountService findAccountService)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _checkLimit = checkLimit;
            _transactionProcessor = transactionProcessor;
            _findAccountService = findAccountService;
        }
        public async Task<TransferResponseDTO> TransferAsync(string appUserId, string iban, decimal amount, string lastNumbers)
        {
            var lookup = await _findAccountService.findAccount(appUserId, lastNumbers);
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
    }
}
