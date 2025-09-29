using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO;
using Domain.Models;

namespace Application.Interfaces.Repositories
{
    public interface IAccountRepository
    {
        Task AddEncyptedPan(Pan pan);
        Task AddCard(Card card);
        Task AddAccount(Account account);
        Task<IEnumerable<GetCardDTO>> GetAllCards(Guid clientId);
        Task<Card> GetRequisitesDTOAsync(Guid clientId, string last_numbers);
        Task<Account> GetAccountById(Guid accountId);
        Task<Account> GetAccountByIban(string iban);
    }
}
