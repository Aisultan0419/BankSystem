using Application.DTO.TransactionDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services.Transactions
{
    public interface IAddingOutboxTransaction
    {
        Task<Guid> ProcessOutboxAddingTransaction(Guid appUserId, SavingsRequestDTO savReqDTO);
    }
}
