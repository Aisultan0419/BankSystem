using Application.Responses;
namespace Application.Interfaces.Services
{
    public interface IFindAccountService
    {
        Task<AccountLookupResult> findAccount(string appUserId, string lastNumbers);
    }
}
