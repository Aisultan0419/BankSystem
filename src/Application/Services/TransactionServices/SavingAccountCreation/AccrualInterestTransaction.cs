using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Domain.Models.Accounts;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class AccrualInterestTransaction : IAccrualInterestTransaction
    {
        private readonly ILogger<AccrualInterestTransaction> _logger;
        private readonly ITransactionRepository _transactionRepository; 
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IExecutionStrategyRunner _exRunner;
        public AccrualInterestTransaction(
            ILogger<AccrualInterestTransaction> logger,
            ITransactionRepository transactionRepository,
            IAccountRepository accountRepository,
            IUserRepository userRepository,
            IExecutionStrategyRunner exRunner)
        {
            _logger = logger;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _exRunner = exRunner;
        }
        public async Task ExecuteAccrualInterest(Account account, decimal rate)
        {
            await _exRunner.ExecuteAsync(async () =>
            {
                using var transaction = await _transactionRepository.BeginTransactionAsync();
                try
                {
                    decimal interest = Math.Round(account.Balance * rate, 2, MidpointRounding.AwayFromZero);
                    bool canDeposit = await IsDayForAccrualInterestDeposit(account);
                    decimal totalAccruedInterest = 0m;
                    if (account is SavingAccount savingAccount)
                    {
                        if (canDeposit is true)
                        {
                            totalAccruedInterest = savingAccount.AccruedInterest;
                            savingAccount.ApplyAccruedInterest();
                        }
                        savingAccount.UpdateInterest(interest);
                    }
                    InterestAccrualHistory interestAccrualHistory = new InterestAccrualHistory
                    {
                        Id = Guid.NewGuid(),
                        AccountId = account.Id,
                        AccrualDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        AmountAccruedToday = interest,
                        AmountAccruedApplied = canDeposit ? totalAccruedInterest : 0m,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _transactionRepository.AddAccrualInterestHistory(interestAccrualHistory);
                    await _userRepository.SaveChangesAsync();
                    await transaction.CommitAsync();
                    _logger.LogInformation("Accrued {Interest} interest to account {AccountId}", interest, account.Id);
                }
                catch (DbUpdateException dbEx) when (IsUniqueViolation(dbEx))
                {
                    await transaction.RollbackAsync();
                    _logger.LogWarning("Unique constraint violation while accruing interest to account {AccountId}. Possible concurrent operation.", account.Id);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error accruing interest to account {AccountId}", account.Id);
                    throw;
                }
            });
        }
        private async Task<bool> IsDayForAccrualInterestDeposit(Account account)
        {
            var canDeposit = await _accountRepository.IsDayForAccrualInterestDeposit(account);
            return canDeposit;
        }
        private bool IsUniqueViolation(DbUpdateException ex)
        {
            bool check = _transactionRepository.IsUniqueViolationCheck(ex);
            return check;
        }
    }
}
