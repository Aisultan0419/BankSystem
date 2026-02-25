using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Accounts;
using Application.Interfaces.Services.AppUsers;
using Application.Interfaces.Services.Auths;
using Application.Interfaces.Services.Cards;
using Application.Interfaces.Services.Clients;
using Application.Interfaces.Services.Pans;
using Application.Interfaces.Services.Transactions;
using Application.Services.AccountServices;
using Application.Services.AppUserServices;
using Application.Services.AuthServices;
using Application.Services.CardServices;
using Application.Services.ClientServices;
using Application.Services.TransactionServices;
using Application.Services.TransactionServices.SavingAccountCreation;
using ApplicationTests.TransactionServicesTests.TransactionTests;
using Domain.Models.Accounts;
using Infrastructure;
using Infrastructure.InterestAccrualService;
using Infrastructure.JWT;
using Infrastructure.MessageBroker;
using Infrastructure.PanServices;
using Infrastructure.Repositories;
using Infrastructure.SavingAccountCreation;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //appuser
            services.AddScoped<IAppUserService, AppUserService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            //auth
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();

            //clients
            services.AddScoped<IClientService, ClientService>();

            //account
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IIbanService, IbanService>();
            services.AddScoped<IFindAccountService, FindAccount>();
            services.AddScoped<ISavingAccountCreationTransaction, SavingAccountCreationTransaction>();
            services.AddScoped<ISavingAccountService, SavingAccountService>();
            services.AddScoped<IInterestAccrualService, InterestAccrualService>();
            services.AddScoped<IAccrualInterestTransaction, AccrualInterestTransaction>();

            //card
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<IGetCardService, GetCardService>();
            services.AddScoped<IGetRequisitesOfCardService, GetRequisitesOfCardService>();
            services.AddScoped<IPanService, Pan_generation>();

            //transactions
            services.AddScoped<ITransferService, TransferService>();
            services.AddScoped<IDepositService, DepositService>();
            services.AddScoped<ITransactionProcessor, TransactionProcessor>();
            services.AddScoped<ITransactionsGetService, TransactionsGetService>();
            services.AddScoped<CheckLimit>();
            services.AddScoped<IAddingOutboxTransaction, AddingOutboxTransaction>();
            services.AddScoped<IOutboxWriter, OutboxWriter>();       
            services.AddScoped<IInterestAccrualService, InterestAccrualService>();       

            //hasher
            services.AddScoped<IHasher, Hasher>();
            
          
            //clock
            services.AddScoped<IClock, SystemClock>();

            //publisher
            services.AddScoped<OutboxPublisher>();
            services.AddScoped<InterestAccrualPublisher>();

            //hosted services
            services.AddHostedService<InterestAccrualPublishService>();
            services.AddHostedService<OutboxHostedService>();

            //repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ISavingAccountService, SavingAccountService>();

            services.AddScoped<IExecutionStrategyRunner, EfExecutionStrategyRunner>();

            return services;
        }

    }
}
