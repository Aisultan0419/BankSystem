using Application.DTO.TransactionDTO;
using Application.MessageContracts;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Transactions
{
    public interface ISavingAccountService
    {
        Task<ApiResponse<SavingAccountResponseDTO>> CreateSavingAccountAsync(string appUserId, SavingsRequestDTO savReqDTO);
        Task CreateSavingAccount(TransactionRequested message);
    }
}
