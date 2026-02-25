using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Accounts;
using Application.Responses;


namespace Application.Services.TransactionServices
{
    public class FindAccount : IFindAccountService
    {
        private readonly IAppUserRepository _appUserRepository;
        private readonly IAccountRepository _accountRepository;
        public FindAccount(IAppUserRepository appUserRepository, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _appUserRepository = appUserRepository;
        }

        public async Task<AccountLookupResult> FindAccountMethod(string appUserId, string lastNumbers)
        {
            if (!Guid.TryParse(appUserId, out var appUserGuid))
                return AccountLookupResult.Fail("Invalid user id format");

            var appUser = await _appUserRepository.GetAppUserAsync(appUserGuid);
            if (appUser == null)
                return AccountLookupResult.Fail("AppUser was not found");

            var card = await _accountRepository.GetRequisitesDTOAsync(appUser.Client.Id, lastNumbers);
            if (card == null)
                return AccountLookupResult.Fail("Card was not found");

            var account = await _accountRepository.GetAccountById(card.AccountId);
            if (account == null)
                return AccountLookupResult.Fail("Account was not found");

            return AccountLookupResult.Ok(account, appUser);
        }
    }
}
