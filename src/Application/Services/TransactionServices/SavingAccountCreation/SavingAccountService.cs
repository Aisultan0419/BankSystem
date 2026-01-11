using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.Transactions;
using Application.MessageContracts;
using Application.Responses;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;
using Microsoft.Extensions.Logging;


namespace Application.Services.TransactionServices.SavingAccountCreation
{
    public class SavingAccountService : ISavingAccountService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly ITransactionRepository _trRepository;
        private readonly IClock _clock;
        private readonly IAddingOutboxTransaction _addOutboxTransaction;
        private readonly IIBanService _iBanService;
        private readonly ILogger<SavingAccountService> _logger;
        private readonly ISavingAccountCreationTransaction _savAccountTransaction;
        private readonly ITransactionProcessor _transactionProcessor;
        public SavingAccountService(IAppUserRepository appUserRepository
            ,ITransactionRepository trRepository
            ,IClock clock
            ,IAddingOutboxTransaction addOutboxTransaction
            ,IIBanService iBanService
            ,ILogger<SavingAccountService> logger
            ,ISavingAccountCreationTransaction savAccountTransaction
            ,ITransactionProcessor transactionProcessor)
        {
            _appUserRepository = appUserRepository;
            _trRepository = trRepository;
            _clock = clock;
            _addOutboxTransaction = addOutboxTransaction;
            _iBanService = iBanService;
            _logger  = logger;
            _savAccountTransaction = savAccountTransaction;
            _transactionProcessor = transactionProcessor;
        }
        public async Task<ApiResponse<SavingAccountResponseDTO>> CreateSavingAccountAsync(string appUserId, SavingsRequestDTO savReqDTO)
        {
            Guid.TryParse(appUserId, out var appUserGuid);
            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);    

            if(appUser is null)
            {
                return new ApiResponse<SavingAccountResponseDTO>
                {
                    IsSuccess = false,
                    Message = "User not found",
                    Error = "The specified user does not exist.",
                    Data = null
                };
            }
            var fromAccount = await _trRepository.GetCurrentAccountByClient(appUser.Client);
            if (fromAccount.Balance < 1000)
            {
                return new ApiResponse<SavingAccountResponseDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid amount",
                    Error = $"The amount must be greater 1000 or equal to it",
                    Data = null
                };
            }
            var corId = await _addOutboxTransaction.ProcessOutboxAddingTransaction(appUser.Id, savReqDTO);

            return new ApiResponse<SavingAccountResponseDTO>
            {
                IsSuccess = true,
                Message = "Saving account creation request processed successfully.",
                Error = null,
                Data = new SavingAccountResponseDTO
                {
                    Status = "Pending",
                    CorrelationId = corId
                }
            };

        }
        

        public async Task CreateSavingAccount(TransactionRequested message)
        {
            var existingMessage = await _trRepository.CheckMessageForIdempotency(message.CorrelationId);
            if (existingMessage is true)
            {
                _logger.LogInformation("Message with CorrelationId {CorrelationId} has already been processed. Skipping.", message.CorrelationId);
                return;
            }

            var interestRate = new InterestRate
            {
                Rate = 0.15m,
                EffectiveFrom = DateOnly.FromDateTime(_clock.UtcNow),
                EffectiveTo = DateOnly.FromDateTime(_clock.UtcNow.AddYears(1)),
                Type = SavingType.Year
            };
            var appUser = await _appUserRepository.GetAppUserAsync(message.UserId);
            var client = appUser.Client;
            
            var iban = await _iBanService.GetIban(AccountType.Deposit, client.Id);
            var savingAccount = new SavingAccount { ClientId = client.Id, Iban = iban }; 
            savingAccount.Initialize(interestRate);
            var fromAccount = await _trRepository.GetCurrentAccountByClient(appUser.Client);
            await _savAccountTransaction.ProcessSavingAccountCreationTransaction(savingAccount);
            await _transactionProcessor.ProcessTransferAsync(appUser, fromAccount, savingAccount, message.Amount);
        }
    }
}
