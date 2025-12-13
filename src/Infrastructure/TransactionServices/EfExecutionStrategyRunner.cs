
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace ApplicationTests.TransactionServicesTests.TransactionTests
{
    public class EfExecutionStrategyRunner : IExecutionStrategyRunner
    {
        private readonly AppDbContext _appDbContext;

        public EfExecutionStrategyRunner(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public Task ExecuteAsync(Func<Task> operation)
        {
            var strategy = _appDbContext.Database.CreateExecutionStrategy();
            return strategy.ExecuteAsync<object, object>(
                null!,
                async (db, state, ct) => { await operation(); return null!; },
                null,
                CancellationToken.None
                );
        }
    }

}
