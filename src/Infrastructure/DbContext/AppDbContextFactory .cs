using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace Infrastructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionBuilder.UseNpgsql("Host = localhost; Port = 5432; Database = BankSystem; Username = postgres; Password = 210624");
            return new AppDbContext(optionBuilder.Options);
        }
    }
}
