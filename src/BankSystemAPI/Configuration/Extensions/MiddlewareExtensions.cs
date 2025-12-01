using BankSystemAPI.Middlewares;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Diagnostics;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMiddlewares(this IApplicationBuilder app)
        {
            app.UseMiddleware<LoggerMiddleware>();
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                Secure = CookieSecurePolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
            });
            app.UseAuthentication();
            app.UseAuthorization();

            return app;
        }
    }
}
