using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.Cards;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;
using System.Runtime.CompilerServices;
namespace Application.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIBanService _ibanService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardService _cardService;
        public AccountService
            (IUserRepository userRepository
            ,IIBanService ibanService
            ,IAccountRepository accountRepository
            ,ICardService cardService)
        {
            _userRepository = userRepository;
            _ibanService = ibanService;
            _accountRepository = accountRepository;
            _cardService = cardService;
        }
        public async Task<CurrentAccount> CreateAccount(Client client)
        {
            var iban = await _ibanService.GetIban(AccountType.Current, client.Id);
            var account = new CurrentAccount
            {
                Id = Guid.NewGuid(),
                Client = client,
                ClientId = client.Id,
                Iban = iban,
            };
            var card = _cardService.CreateCard(client);
            card.Account = account;
            card.AccountId = account.Id;
            account.Cards.Add(card);
            return account;
        }
    }
}
