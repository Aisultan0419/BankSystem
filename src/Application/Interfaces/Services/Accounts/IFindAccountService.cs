using Application.Responses;
namespace Application.Interfaces.Services.Accounts
{
    public interface IFindAccountService
    {
        Task<AccountLookupResult> FindAccountMethod(string appUserId, string lastNumbers);
    }
}
