using Application.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services.AccountServices;
using Application.Services.AppUserServices;
using Application.Services.AuthServices;
using Application.Services.CardServices;
using Application.Services.ClientServices;
using Application.Services.TransactionServices;
using Application.Validators.AppUserValidators;
using Application.Validators.CardValidators;
using Application.Validators.ClientValidators;
using Application.Validators.TransactionQueryValidators;
using BankSystemAPI.Extensions;
using Domain.Configuration;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.DbContext; 
using Infrastructure.JWT;
using Infrastructure.PanServices;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bank API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "¬ведите токен в формате: Bearer {твой токен}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IHasher, Hasher>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IGetRequisitesOfCardService, GetRequisitesOfCardService>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IGetCardService, GetCardService>();
builder.Services.AddScoped<IPanService, Pan_generation>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDepositService, DepositService>();
builder.Services.AddScoped<IIBanService, IbanService>();
builder.Logging.AddConsole();

builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<ClientValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AppUserValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginViaPasswordValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginViaPinValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LastNumberValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<IINValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransferQueryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<DepositQueryValidator>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<RefreshTokenOptions>(
        builder.Configuration.GetSection("RefreshJwtOptions")
    );
builder.Services.Configure<JwtOptions>(opts =>
{
    builder.Configuration.GetSection("JwtOptions").Bind(opts);
    opts.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
        ?? throw new InvalidOperationException("Jwt secret not set");
});

builder.Services.AddApiAuthentication(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>  
    options.UseNpgsql("Host = localhost; Port = 5432; Database = BankSystem; Username = postgres; Password = 210624"));
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IRefreshTokenProvider, RefreshTokenProvider>();
builder.Services.AddSingleton<IPanEncryptor>(sp =>
{
    var keyString = builder.Configuration["PanSecretKey:Key"]
                    ?? throw new ArgumentNullException("Pan secret key was not found");

    var key = Convert.FromBase64String(keyString);
    return new PanEncryptor(key);
});
var app = builder.Build();
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "My Bank API");
        });
}
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.Strict,
});
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
