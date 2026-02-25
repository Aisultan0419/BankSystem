using Domain.Models;
using Domain.Enums;
using Application.Interfaces.Services.Cards;
using Application.Interfaces.Services.Pans;
namespace Application.Services.CardServices
{
    public class CardService : ICardService
    {
        private readonly IPanService _panService;
        private readonly IPanEncryptor _panEncryptor;
        public CardService(IPanService panService
            ,IPanEncryptor panEncryptor)
        {
            _panService = panService;
            _panEncryptor = panEncryptor;
        }
        public Card CreateCard(Client client)
        {
            var pan = _panService.CreatePan(client);
            var encrypted = _panEncryptor.Encrypt(pan);

            var encrypted_pan = new Pan
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
                Pan = encrypted_pan
            };
            encrypted_pan.Card = card;
            return card;
        }

    }
}
