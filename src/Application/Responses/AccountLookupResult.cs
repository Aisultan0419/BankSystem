using Domain.Models;
using Domain.Models.Accounts;

namespace Application.Responses
{
    public class AccountLookupResult
    {
        public bool Success { get; private set; }
        public string? ErrorMessage { get; private set; }
        public Account? Account { get; private set; }
        public AppUser? AppUser { get; private set; }

        private AccountLookupResult() { }

        public static AccountLookupResult Fail(string message) =>
            new AccountLookupResult { Success = false, ErrorMessage = message };

        public static AccountLookupResult Ok(Account account, AppUser appUser) =>
            new AccountLookupResult { Success = true, Account = account, AppUser = appUser };
    }
}
