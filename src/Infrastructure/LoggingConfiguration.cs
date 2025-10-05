using Serilog;
namespace Infrastructure
{
    public static class LoggingConfiguration 
    {
        public static ILogger CreateLogger(string logFilePath)
        {
            return new LoggerConfiguration()
                .MinimumLevel.Information() 
                .WriteTo.Console()
                .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
                .Enrich.WithThreadId()
                .Enrich.WithProcessId()
                .Enrich.WithMachineName()
                .CreateLogger();
        }
    }
}
