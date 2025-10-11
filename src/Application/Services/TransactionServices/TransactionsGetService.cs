using Application.DTO.TransactionDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Responses;
namespace Application.Services.TransactionServices
{
    public class TransactionsGetService : ITransactionsGetService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly ITransactionRepository _transactionRepository;
        public TransactionsGetService(IAppUserRepository appUserRepository, ITransactionRepository transactionRepository)
        {
            _appUserRepository = appUserRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<ApiResponse<List<TransactionsGetDTO>>> GetAllTransactionsAsync(string appUserId)
        {
            Guid.TryParse(appUserId, out var appUserGuid);
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

            var result = await _transactionRepository.GetTransactions(client);
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
