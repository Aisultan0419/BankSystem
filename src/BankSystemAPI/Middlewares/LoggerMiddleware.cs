using System.Diagnostics;

namespace BankSystemAPI.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly ILogger<LoggerMiddleware> _logger;
        private readonly RequestDelegate _next;
        public LoggerMiddleware(ILogger<LoggerMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var method = context.Request.Method;
                var path = context.Request.Path;
                var host = context.Request.Headers["Host"].ToString();
                _logger.LogInformation("Incoming Request: {Method} {Path} {host}", method, path, host);
                await _next(context);
                stopwatch.Stop();
                var statusCode = context.Response.StatusCode;
                _logger.LogInformation("Outgoing Response: {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms", method, path, statusCode, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var method = context.Request.Method;
                var path = context.Request.Path;
                _logger.LogError(ex, "An unhandled exception occurred while processing the request {method} - {path} in {ElapsedMilliseconds}ms", method, path, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
