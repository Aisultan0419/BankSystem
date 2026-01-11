using Application.Responses;
namespace Application.Interfaces.Services.Accounts
{
    public interface IFindAccountService
    {
        Task<AccountLookupResult> findAccount(string appUserId, string lastNumbers);
    }
}
