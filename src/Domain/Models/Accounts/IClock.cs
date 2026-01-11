
namespace Domain.Models.Accounts
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}
