using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;

namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class SavingAccountCreationTransaction : ISavingAccountCreationTransaction
    {
        private readonly ITransactionRepository _tR;
        private readonly IUserRepository _userRepository;
        private readonly IExecutionStrategyRunner _exRunner;
        private readonly IAccountRepository _accountRepository;
        public SavingAccountCreationTransaction(
            ITransactionRepository tr,
            IUserRepository userRep,
            IExecutionStrategyRunner exRunner,
            IAccountRepository accountRepo)
        {
            _userRepository = userRep;
            _tR = tr;
            _exRunner = exRunner;
            _accountRepository = accountRepo;
        }
       
        public async Task ProcessSavingAccountCreationTransaction(SavingAccount savingAccount)
        {
            await _exRunner.ExecuteAsync(async () =>
            { 
                await using var tx = await _tR.BeginTransactionAsync();
                await _accountRepository.AddSavingAccount(savingAccount);
                await _userRepository.SaveChangesAsync();
                await tx.CommitAsync();
            });
        }
    }
}
