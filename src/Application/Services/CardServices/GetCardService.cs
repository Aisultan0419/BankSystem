using Application.DTO.CardDTO;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
namespace Application.Services.CardServices
{
    public class GetCardService : IGetCardService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        public GetCardService(IAppUserRepository appUserRepository, IAccountRepository accountRepository)
        {
            _appUserRepository = appUserRepository;
            _accountRepository = accountRepository;
        }
        public async Task<IEnumerable<GetCardDTO>> GetAllCards(string appUserId)
        {
            var appUser = await _appUserRepository.GetAppUserAsync(Guid.Parse(appUserId)) ?? throw new Exception();

            var client = appUser.Client;

            var result = await _accountRepository.GetAllCards(client.Id) ?? throw new Exception();

            return result;
        }
    }
}
