using Application.DTO.TransactionDTO;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Transactions;

public interface IPurchaseService
{
    Task<ApiResponse<PurchaseResponseDTO>> PurchaseAsync(PurchaseQueryDTO query, string appUserId);
}
