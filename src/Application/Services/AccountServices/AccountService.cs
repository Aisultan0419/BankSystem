using Application.Interfaces.Repositories;
using Domain.Models;
using Domain.Enums;
using Application.Interfaces.Services;
namespace Application.Services.AccountServices
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IIBanService _ibanService;
        private readonly IAccountRepository _accountRepository;
        private readonly ICardService _cardService;
        public AccountService(IUserRepository userRepository
            ,IIBanService ibanService
            ,IAccountRepository accountRepository
            ,ICardService cardService)
        {
            _userRepository = userRepository;
            _ibanService = ibanService;
            _accountRepository = accountRepository;
            _cardService = cardService;
        }

        public async Task CreateAccount(Client client)
        {
            var iban = await _ibanService.GetIban(AccountType.Current, client.Id);
            var account = new Account
            {
                Id = Guid.NewGuid(),
                Client = client,
                Iban = iban,
            };
            var card = await _cardService.CreateCard(client);
            account.Cards.Add(card);
            await _accountRepository.AddAccount(account);
            await _userRepository.SaveChangesAsync();
        }
    }
}
