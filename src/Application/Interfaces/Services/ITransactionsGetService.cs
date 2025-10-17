using Application.DTO.TransactionDTO;
using Application.Responses;

namespace Application.Interfaces.Services
{
    public interface ITransactionsGetService
    {
        Task<ApiResponse<List<TransactionsGetDTO>>> GetAllTransactionsAsync(string appUserId, TransactionHistoryQueryDTO thqDTO);
    }
}
