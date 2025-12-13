using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services.AccountServices;
using Application.Services.AppUserServices;
using Application.Services.AuthServices;
using Application.Services.CardServices;
using Application.Services.ClientServices;
using Application.Services.TransactionServices;
using ApplicationTests.TransactionServicesTests.TransactionTests;
using Infrastructure;
using Infrastructure.JWT;
using Infrastructure.PanServices;
using Infrastructure.Repositories;
using System.Reflection.Metadata.Ecma335;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IDepositService, DepositService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IHasher, Hasher>();
            services.AddScoped<IGetCardService, GetCardService>();
            services.AddScoped<IGetRequisitesOfCardService, GetRequisitesOfCardService>();
            services.AddScoped<ITransactionProcessor, TransactionProcessor>();
            services.AddScoped<ITransactionsGetService, TransactionsGetService>();
            services.AddScoped<IIBanService, IbanService>();
            services.AddScoped<CheckLimit>();
            services.AddScoped<IPanService, Pan_generation>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();



            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();

            services.AddScoped<IExecutionStrategyRunner, EfExecutionStrategyRunner>();


            return services;
        }

    }
}
