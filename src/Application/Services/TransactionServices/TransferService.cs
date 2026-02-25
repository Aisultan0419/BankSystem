using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.AppUsers;
using Application.Interfaces.Services.Transactions;
namespace Application.Services.TransactionServices
{
    public class TransferService : ITransferService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionProcessor _transactionProcessor;
        private readonly CheckLimit _checkLimit;
        private readonly IFindAccountService _findAccountService;
        public TransferService(
             IAccountRepository accountRepository
            ,CheckLimit checkLimit
            ,ITransactionProcessor transactionProcessor
            ,IFindAccountService findAccountService
            ,ICurrentUserService currentUserService)
        {
            _accountRepository = accountRepository;
            _checkLimit = checkLimit;
            _transactionProcessor = transactionProcessor;
            _findAccountService = findAccountService;
            _currentUserService = currentUserService;
        }
        public async Task<TransferResponseDTO> TransferAsync(string iban, decimal amount, string lastNumbers)
        {
            var appUserId = _currentUserService.GetUserId();
            var lookup = await _findAccountService.FindAccountMethod(appUserId.ToString(), lastNumbers);
            if (!lookup.Success)
                return new TransferResponseDTO { Message = lookup.ErrorMessage };

            var fromAccount = lookup.Account!;
            var appUser = lookup.AppUser!;
            if (fromAccount.Balance < amount)
            {
                return new TransferResponseDTO
                {
                    Message = "Insufficient funds"
                };
            }
            var resultOfCheck = _checkLimit.CheckDailyTransferLimit(fromAccount, amount);
            if (resultOfCheck.Item1 == false)
            {
                return new TransferResponseDTO
                {
                    Message = $"You can transfer only {resultOfCheck.Item2} for today"
                };
            }
            
            var toAccount = await _accountRepository.GetAccountByIban(iban);
            if (toAccount == null)
            {
                return new TransferResponseDTO
                {
                    Message = "Recipient account was not found"
                };
            }
            await _transactionProcessor.ProcessTransferAsync(appUser, fromAccount, toAccount, amount);

            return new TransferResponseDTO
            {
                FullName = toAccount.Client.FullName,
                TransferredAmount = amount,
                RemainingBalance = fromAccount.Balance
            };
        }
    }
}
