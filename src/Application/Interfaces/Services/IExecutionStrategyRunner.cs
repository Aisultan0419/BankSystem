
namespace ApplicationTests.TransactionServicesTests.TransactionTests
{
    public interface IExecutionStrategyRunner
    {
        Task ExecuteAsync(Func<Task> operation);
    }
}
