using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Domain.Models.Accounts;
using Microsoft.Extensions.Logging;

namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class InterestAccrualService : IInterestAccrualService
    {
        private readonly ILogger<InterestAccrualService> _logger;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAccrualInterestTransaction _accrualInterestTransaction;
        private readonly IAccountRepository _accountRepository;
        public InterestAccrualService(ILogger<InterestAccrualService> logger
            ,ITransactionRepository transactionRepository
            ,IUserRepository userRepository
            ,IAccrualInterestTransaction accrualInterestTransaction
            ,IAccountRepository accountRepository)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _userRepository = userRepository;
            _accrualInterestTransaction = accrualInterestTransaction;
            _accountRepository = accountRepository;
        }
        public async Task AccrualInterest(AccrueInterestCommand command)
        {
            var account = await _accountRepository.GetAccountById(command.AccountId);
            decimal interestRate;
            if (account is SavingAccount savingAccount)
            {
                interestRate = savingAccount._interestRate!.Rate;
            }
            else
            {
                throw new InvalidOperationException("Account is not a SavingAccount");
            }
            var dailyInterestRate = interestRate / 365;
            await _accrualInterestTransaction.ExecuteAccrualInterest(account, dailyInterestRate);
        }
    }
}
