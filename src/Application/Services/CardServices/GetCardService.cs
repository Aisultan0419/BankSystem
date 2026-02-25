using Application.DTO.CardDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Responses;
using Application.Interfaces.Services.AppUsers;
namespace Application.Services.CardServices
{
    public class GetCardService : IGetCardService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ICurrentUserService _currentUserService;   
        public GetCardService(IAppUserRepository appUserRepository
            ,IAccountRepository accountRepository
            ,ICurrentUserService currentUserService)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
            _currentUserService = currentUserService;
        }
        public async Task<ApiResponse<IEnumerable<GetCardDTO>>> GetAllCards()
        {
            Guid appUserId = _currentUserService.GetUserId();
            var appUser = await _appUserRepository.GetAppUserAsync(appUserId);
            if(appUser is null)
            {
                return new ApiResponse<IEnumerable<GetCardDTO>>
                {
                    IsSuccess = false,
                    Message = "App user not found",
                    Error = "APP_USER_NOT_FOUND",
                    Data = null
                };
            }
            var client = appUser.Client;
            var result = await _accountRepository.GetAllCards(client.Id);
            if (result is null)
            {
                return new ApiResponse<IEnumerable<GetCardDTO>>
                {
                    IsSuccess = false,
                    Message = "No cards found for the client",
                    Error = "NO_CARDS_FOUND",
                    Data = null
                };
            }
            return new ApiResponse<IEnumerable<GetCardDTO>>
            {
                IsSuccess = true,
                Message = "Cards retrieved successfully",
                Error = null,
                Data = result
            };
        }
    }
}
