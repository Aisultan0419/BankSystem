namespace BankSystemAPI.Configuration.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseApiSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "My Bank API");
            });
            return app;
        }
    }
}
