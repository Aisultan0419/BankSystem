using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.CardDTO;
using Domain.Models;
using Domain.Models.Accounts;

namespace Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task<IEnumerable<GetCardDTO>?> GetAllCards(Guid clientId);
        Task<Card> GetRequisitesDTOAsync(Guid clientId, string lastNumbers);
        Task<Account> GetAccountById(Guid accountId);
        Task<Account> GetAccountByIban(string iban);
        Task<bool> IsDayForAccrualInterestDeposit(Account account);
        Task<bool> CheckForIdempotencyOfAccrualInterest(Account account);
        Task<string?> GetOrderNumber();
    }
}
