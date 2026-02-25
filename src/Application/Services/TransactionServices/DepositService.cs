using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.AppUsers;
using Application.Interfaces.Services.Transactions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace Application.Services.TransactionServices
{
    public class DepositService : IDepositService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountRepository _accountRepository;
        private readonly CheckLimit _checkLimit;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IExecutionStrategyRunner _exRunner;
        private readonly ICurrentUserService _currentUserService;
        private static readonly TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Asia/Almaty");
        private DateTime KazNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
        private DateOnly KazToday => DateOnly.FromDateTime(KazNow);
        public DepositService(
            IAppUserRepository appUserRepository,
            IUnitOfWork unitOfWork,
            IAccountRepository accountRepository,
            ITransactionRepository transactionRepository,
            CheckLimit checkLimit,
            IExecutionStrategyRunner exRunner,
            ICurrentUserService currentUserService)
        {
            _appUserRepository = appUserRepository;
            _unitOfWork = unitOfWork;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _checkLimit = checkLimit;
            _exRunner = exRunner;
            _currentUserService = currentUserService;
        }
        private const decimal daily_limit = 2000000m;
        public async Task<DepositResponseDTO> DepositAsync(decimal amount, string lastNumbers)
        {
            var appUserId = _currentUserService.GetUserId();    
            var appUser = await _appUserRepository.GetAppUserAsync(appUserId);
            if (appUser == null)
            {
                return new DepositResponseDTO
                {
                    Message = "AppUser was not found"
                };
            }
            if (isValidAmount(amount) == false) return new DepositResponseDTO { Message = "Invalid amount" };
            var clientId = appUser.Client.Id;
            var card = await _accountRepository.GetRequisitesDTOAsync(clientId, lastNumbers);
            if (card == null)
            {
                return new DepositResponseDTO
                {
                    Message = "Card was not found"
                };
            }
            var account = await _accountRepository.GetAccountById(card.AccountId);
            var resultOfCheck = _checkLimit.CheckDailyTransferLimit(account, amount);
            if (resultOfCheck.Item1 == false)
            {
                return new DepositResponseDTO
                {
                    Message = $"You can deposit only {resultOfCheck.Item2}"
                };
            }
            await _exRunner.ExecuteAsync(async () =>
            {
                await using (var tx = await _transactionRepository.BeginTransactionAsync())
                {
                    try
                    {
                        account.Deposit(amount);
                        account.DepositedLastDay += amount;
                        account.LastDepositDateKz = KazToday;
                        var transaction = new Transaction
                        {
                            Id = Guid.NewGuid(),
                            From = "External",
                            To = account.Iban.ToString(),
                            ClientId = appUser.Client.Id,
                            Amount = amount,
                            CreatedAt = DateTime.UtcNow,
                            Type = "Deposit"
                        };
                        await _unitOfWork.AddItem(transaction);
                        await _unitOfWork.SaveChangesAsync();

                        await tx.CommitAsync();
                    }
                    catch
                    {
                        await tx.RollbackAsync();
                        throw;
                    }
                }
            });
            return new DepositResponseDTO
            {
                Message = "Balance is succesfully replenished",
                DepositedAmount = amount,
                NewBalance = account.Balance
            };
        }
        private bool isValidAmount(decimal amount) => amount > 0 && amount <= daily_limit;
    }
}
