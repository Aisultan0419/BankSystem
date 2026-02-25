using Application.Interfaces.Repositories;
using Infrastructure.DbContext;
namespace Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task AddItem<T>(T item) where T : class
        {
            await _dbContext.Set<T>().AddAsync(item);
        }
    }
}
