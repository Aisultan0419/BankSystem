using Application.Validators.AccountValidators;
using Application.Validators.AppUserValidators;
using Application.Validators.CardValidators;
using Application.Validators.ClientValidators;
using Application.Validators.TransactionQueryValidators;
using FluentValidation;

namespace BankSystemAPI.Configuration.Extensions
{
    public static class ApplicationValidatorsExtensions
    {
        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<ClientValidator>();
            services.AddValidatorsFromAssemblyContaining<AppUserValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginViaPasswordValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginViaPinValidator>();
            services.AddValidatorsFromAssemblyContaining<LastNumberValidator>();
            services.AddValidatorsFromAssemblyContaining<IINValidator>();
            services.AddValidatorsFromAssemblyContaining<TransactionGetHistoryValidator>();
            services.AddValidatorsFromAssemblyContaining<TransferQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<DepositQueryValidator>();
            services.AddValidatorsFromAssemblyContaining<AccountGenerationValidator>();


            return services;
        }
    }
}

