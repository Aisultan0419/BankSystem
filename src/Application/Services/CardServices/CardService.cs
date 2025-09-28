using Domain.Models;
using Domain.Enums;
using Application.Interfaces.Services;
using Application.Interfaces;
using Application.Interfaces.Repositories;
namespace Application.Services.CardServices
{
    public class CardService : ICardService
    {
        private readonly IPanService _panService;
        private readonly IPanEncryptor _panEncryptor;
        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        public CardService(IPanService panService
            ,IPanEncryptor panEncryptor
            ,IAccountRepository accountRepository
            ,IUserRepository userRepository)
        {
            _panService = panService;
            _panEncryptor = panEncryptor;
            _accountRepository = accountRepository;
            _userRepository = userRepository;
        }
        public Card CreateCard(Client client)
        {
            var pan = _panService.CreatePan(client);
            var encrypted = _panEncryptor.Encrypt(pan);
            var encypted_pan = new Pan
            {
                Id = Guid.NewGuid(),
                CipherText = encrypted.CipherText,
                Nonce = encrypted.Nonce,
                Tag = encrypted.Tag
            };
            
            var card = new Card
            {
                Id = Guid.NewGuid(),
                ExpiryDate = (DateTime.UtcNow.AddYears(3)).ToString("MM/yy"),
                Status = CardStatus.Active,
                PanMasked = new string('*', 12) + pan[^4..],
                Pan = encypted_pan
            };
            encypted_pan.Card = card;
            return card;
        }

    }
}
