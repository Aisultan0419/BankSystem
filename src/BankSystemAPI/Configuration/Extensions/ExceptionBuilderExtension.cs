using Microsoft.AspNetCore.Diagnostics;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class ExceptionBuilderExtension
    {
        public static WebApplication UseGlobalExceptionHandler(this WebApplication app)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";

                    var box = context.Features.Get<IExceptionHandlerPathFeature>();
                    var error = box?.Error;

                    logger.LogError(error, "Unhandled exception: {message}", error?.Message);

                    var Error = new
                    {
                        Topic = "Unhandled exception",
                        message = error?.Message
                    };

                    string json = System.Text.Json.JsonSerializer.Serialize(Error);
                    await context.Response.WriteAsync(json);
                });
            });
            return app;
        }
    }
}
