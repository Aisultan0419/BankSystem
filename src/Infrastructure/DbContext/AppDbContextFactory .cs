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
            var conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
           ?? "Host=localhost;Port=5432;Database=BankSystemDB;Username=postgres;Password=210624";
            optionBuilder.UseNpgsql(conn);
            return new AppDbContext(optionBuilder.Options);
        }
    }
}
