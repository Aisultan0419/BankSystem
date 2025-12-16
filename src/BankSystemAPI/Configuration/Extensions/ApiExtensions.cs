using System.Text;
using Domain.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class ApiExtensions
    {
        public static void AddApiAuthentication(this IServiceCollection services, IConfiguration configuration
            ,IHostEnvironment env)
        {
            var section = configuration.GetSection("JwtOptions");
            services.Configure<JwtOptions>(section);

            var jwtOptions = section.Get<JwtOptions>() ?? new JwtOptions();
            if (env.IsDevelopment())
            {
                jwtOptions.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                                        ?? throw new InvalidOperationException("Jwt secret not set");
            }
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine($"[AUTH FAILED] {ctx.Exception.Message}");
                            return Task.CompletedTask;
                        },
                        OnChallenge = ctx =>
                        {
                            Console.WriteLine("[AUTH CHALLENGE] Token missing or invalid");
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = ctx =>
                        {
                            Console.WriteLine("[AUTH SUCCESS] Token validated");
                            return Task.CompletedTask;
                        }
                    };
                });
            
            services.AddAuthorization();
        }
    }
}
