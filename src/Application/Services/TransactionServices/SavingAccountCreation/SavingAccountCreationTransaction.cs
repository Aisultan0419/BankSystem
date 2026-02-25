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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IExecutionStrategyRunner _exRunner;
        private readonly IAccountRepository _accountRepository;

        public SavingAccountCreationTransaction(
            ITransactionRepository tR,
            IUnitOfWork unitOfWork,
            IExecutionStrategyRunner exRunner,
            IAccountRepository accountRepository)
        {
            _unitOfWork = unitOfWork;
            _tR = tR;
            _exRunner = exRunner;
            _accountRepository = accountRepository;
        }

        public async Task ProcessSavingAccountCreationTransaction(SavingAccount savingAccount)
        {
            await _exRunner.ExecuteAsync(async () =>
            { 
                await using var tx = await _tR.BeginTransactionAsync();
                await _unitOfWork.AddItem(savingAccount);
                await _unitOfWork.SaveChangesAsync();
                await tx.CommitAsync();
            });
        }
    }
}
