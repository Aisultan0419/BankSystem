using Application.MessageContracts;
using Infrastructure.MessageBroker;
using Infrastructure.SavingAccountCreation;
using MassTransit;
using MassTransit.Transports.Fabric;
namespace BankSystemAPI.Configuration.Extensions
{
    public static class RabbitMqBusConfigurator 
    {
        public static IServiceCollection AddRabbitMq(this IServiceCollection services)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<TransactionRequestedConsumer>();
                x.AddConsumer<AccrualnterestConsumer>();


                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", "/", h =>
                    {
                        h.Username("guest");
                        h.Password("guest");
                    });


                    cfg.ReceiveEndpoint("account-service.saving.requests", e =>
                    {
                        e.ConfigureConsumer<TransactionRequestedConsumer>(context);

                        e.Bind("transaction.topic", s =>
                        {
                            s.ExchangeType = "topic";
                            s.RoutingKey = "transaction.requested.v1.saving";
                        });

                        e.PrefetchCount = 16;                 
                        e.ConcurrentMessageLimit = 8;         
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5))); 
                    });

                    cfg.ReceiveEndpoint("account-service.accrual-interest.requests", e =>
                    {
                        e.ConfigureConsumer<AccrualnterestConsumer>(context);

                        e.Bind("accrual-interest.topic", s =>
                        {
                            s.ExchangeType = "topic";
                            s.RoutingKey = "accrual-interest.requested.v1.saving";
                        });
                        e.Bind<AccrueInterestCommand>(b =>
                        {
                            b.ExchangeType = "fanout"; 
                        });
                        e.PrefetchCount = 16;
                        e.ConcurrentMessageLimit = 8;
                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    });
                });
            });
            return services;
        }

    }
}
