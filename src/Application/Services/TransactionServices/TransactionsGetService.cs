using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.AppUsers;
using Application.Interfaces.Services.Transactions;
using Application.Responses;
namespace Application.Services.TransactionServices
{
    public class TransactionsGetService : ITransactionsGetService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ICurrentUserService _currentUserService;
        public TransactionsGetService(IAppUserRepository appUserRepository
            ,ITransactionRepository transactionRepository
            ,ICurrentUserService currentUserService)
        {
            _appUserRepository = appUserRepository; 
            _transactionRepository = transactionRepository;
            _currentUserService = currentUserService;
        }
        public async Task<ApiResponse<List<TransactionsGetDTO>>> GetAllTransactionsAsync(TransactionHistoryQueryDTO thqDTO)
        {
            var appUserGuid = _currentUserService.GetUserId();
            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
            {
                return new ApiResponse<List<TransactionsGetDTO>>
                {
                    IsSuccess = false,
                    Message = "AppUser was not found",
                    Data = null
                };
            }
            var client = appUser.Client;

            var result = await _transactionRepository.GetTransactions(client, thqDTO);
            if (result == null)
            {
                return new ApiResponse<List<TransactionsGetDTO>>
                {
                    IsSuccess = false,
                    Message = "No transactions found",
                    Data = null
                };
            }
            return new ApiResponse<List<TransactionsGetDTO>>
            {
                IsSuccess = true,
                Message = "Transactions retrieved successfully",
                Data = result
            };
        }
    }
}
