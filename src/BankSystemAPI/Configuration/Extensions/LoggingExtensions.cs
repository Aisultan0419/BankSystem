using Infrastructure;
using Serilog;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class LoggingExtensions
    {
        public static void AddApiLogging(this IServiceCollection services, IConfiguration configuration)
        {
            Log.Logger = LoggingConfiguration.CreateLogger("Logs/log-.txt");
        }
        public static void UseApiLogging(this IHostBuilder host)
        {
            host.UseSerilog();
        }
    }
}
