using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyDatabaseMigrations(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                try
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var strategy = db.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        var maxAttempts = 10;
                        for (int i = 1; i <= maxAttempts; i++)
                        {
                            try
                            {
                                await db.Database.OpenConnectionAsync();
                                await db.Database.CloseConnectionAsync();
                                break;
                            }
                            catch
                            {
                                if (i == maxAttempts) throw;
                                await Task.Delay(2000);
                            }
                        }

                        await db.Database.MigrateAsync();
                    });

                    logger.LogInformation("Database migrations applied.");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while migrating or initializing the database.");
                    throw;
                }
            }
        }
    }

}
