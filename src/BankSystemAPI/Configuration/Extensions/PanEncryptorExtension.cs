using Application.Interfaces;
using Infrastructure.PanServices;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class PanEncryptorExtension
    {
        public static IServiceCollection AddPanEncryptor(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPanEncryptor>(sp =>
            {
                var keyString = configuration["PanSecretKey:Key"]
                                ?? throw new ArgumentNullException("Pan secret key was not found");

                var key = Convert.FromBase64String(keyString);
                return new PanEncryptor(key);
            });
            return services;
        }
    }
}
