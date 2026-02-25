using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Domain.Models.Accounts;
using Microsoft.Extensions.Logging;

namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class InterestAccrualService : IInterestAccrualService
    {
        private readonly IAccrualInterestTransaction _accrualInterestTransaction;
        private readonly IAccountRepository _accountRepository;

        public InterestAccrualService(
            IAccrualInterestTransaction accrualInterestTransaction,
            IAccountRepository accountRepository)
        {
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
