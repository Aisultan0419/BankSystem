using Application.DTO.CardDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.AppUsers;
using Application.Interfaces.Services.Cards;
using Application.Interfaces.Services.Pans;
using Application.Responses;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Application.Services.CardServices
{
    public class GetRequisitesOfCardService : IGetRequisitesOfCardService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPanEncryptor _panEncryptor;
        private readonly ICurrentUserService _currentUserService;
        public GetRequisitesOfCardService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,IPanEncryptor panEncryptor
            ,ICurrentUserService currentUserService)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _panEncryptor = panEncryptor;
            _currentUserService = currentUserService;
        }
        public async Task<ApiResponse<CardRequisitesDTO>> GetRequisitesOfCard(string last_numbers)
        {
            var appUserIdGuid = _currentUserService.GetUserId();
            var appUser = await _appUserRepository.GetAppUserAsync(appUserIdGuid);
            if (appUser is null)
            {
                return new ApiResponse<CardRequisitesDTO>
                {
                    IsSuccess = false,
                    Message = "AppUser was not found",
                    Error = "APP_USER_NOT_FOUND",
                    Data = null
                };
            }

            var clientId = appUser.Client.Id;
            var card = await _accountRepository.GetRequisitesDTOAsync(clientId, last_numbers);

            string pan_string;
            try
            {
                pan_string = _panEncryptor.Decrypt(card.Pan.CipherText, card.Pan.Nonce, card.Pan.Tag);
            }
            catch (CryptographicException)
            {
                return new ApiResponse<CardRequisitesDTO>
                {
                    IsSuccess = false,
                    Message = "Invalid Pan",
                    Error = "INVALID_PAN",
                    Data = new CardRequisitesDTO
                    {
                        FullName = appUser.Client.FullName!,
                        Pan = "Invalid Pan",
                        ExpiryDate = card.ExpiryDate
                    }
                };
            }
            return new ApiResponse<CardRequisitesDTO>
            {
                IsSuccess = true,
                Message = "Card requisites retrieved successfully",
                Error = null,
                Data = new CardRequisitesDTO
                {
                    FullName = appUser.Client.FullName!,
                    Pan = pan_string,
                    ExpiryDate = card.ExpiryDate
                }
            };
        }

    }
}
