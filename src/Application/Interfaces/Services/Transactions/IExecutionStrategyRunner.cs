namespace Application.Interfaces.Services.Transactions
{
    public interface IExecutionStrategyRunner
    {
        Task ExecuteAsync(Func<Task> operation);
    }
}
