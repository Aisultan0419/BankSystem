using System;
using System.Collections.Generic;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.DTO.CardDTO;

namespace Application.Services.CardServices
{
    public class GetRequisitesOfCardService : IGetRequisitesOfCardService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPanEncryptor _panEncryptor;
        public GetRequisitesOfCardService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,IPanEncryptor panEncryptor)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _panEncryptor = panEncryptor;
        }
        public async Task<CardRequisitesDTO> GetRequisitesOfCard(string appUserId, string last_numbers)
        {
            Guid.TryParse(appUserId, out var appUserIdGuid);

            var appUser = await _appUserRepository.GetAppUserAsync(appUserIdGuid) ?? throw new Exception("AppUser was not found");

            var clientId = appUser.Client.Id;

            var card = await _accountRepository.GetRequisitesDTOAsync(clientId, last_numbers);

            var pan_string = _panEncryptor.Decrypt(card.Pan.CipherText, card.Pan.Nonce, card.Pan.Tag);

            var result = new CardRequisitesDTO { 
                full_name = appUser.Client.FullName!,
                Pan = pan_string,
                ExpiryDate = card.ExpiryDate
            };
            return result;

        }
    }
}
