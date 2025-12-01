using FluentValidation;
using FluentValidation.AspNetCore;
using Domain.Configuration;
using Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
namespace BankSystemAPI.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiBasics(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();


            services.AddSwaggerWithJwt();
            services.AddHttpClient();
            services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();

            services.AddValidatorsFromAssemblyContaining<Program>();
            services.AddHttpContextAccessor();

            services.Configure<JwtOptions>(opts =>
            {
                configuration.GetSection("JwtOptions").Bind(opts);
                opts.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                    ?? throw new InvalidOperationException("Jwt secret not set");
            });
            services.Configure<RefreshTokenOptions>(
                configuration.GetSection("RefreshJwtOptions")
            );

            services.AddApplicationServices();
            services.AddApplicationValidators();

            services.AddDbContext<AppDbContext>(options =>
            {
                var conn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                   ?? configuration.GetConnectionString("DefaultConnection")
                   ?? throw new InvalidOperationException("Connection string not found. Set ConnectionStrings:DefaultConnection or env ConnectionStrings__DefaultConnection");
                options.UseNpgsql(conn, npg =>
                {
                    npg.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                });
            });
            return services;
        }
    }
}
