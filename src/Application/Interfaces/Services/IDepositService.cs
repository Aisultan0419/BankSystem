using Application.DTO.TransactionDTO;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IDepositService
    {
        Task<DepositResponseDTO> DepositAsync(decimal amount, string appUserId, string lastNumbers);
    }
}
