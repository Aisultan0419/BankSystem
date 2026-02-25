using Application.DTO.TransactionDTO;
using Application.Responses;

namespace Application.Interfaces.Services.Transactions
{
    public interface ITransactionsGetService
    {
        Task<ApiResponse<List<TransactionsGetDTO>>> GetAllTransactionsAsync(TransactionHistoryQueryDTO thqDTO);
    }
}
