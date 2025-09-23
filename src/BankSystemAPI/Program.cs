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
using BankSystem;
using Domain.Configuration;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.JWT;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAppUserService, AppUserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAppUserRepository, AppUserRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IPanService, Pan_generation>();
builder.Services.AddScoped<IIBanService, IbanService>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<RefreshTokenOptions>(
        builder.Configuration.GetSection("RefreshJwtOptions")
    );
builder.Services.Configure<JwtOptions>(options => 
{
    builder.Configuration.GetSection("JwtOptions").Bind(options);
    options.SecretKey = Environment.GetEnvironmentVariable("JWT_SECRET")
                                ?? throw new InvalidOperationException("Jwt secret not set");
});

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
app.UseAuthorization();
app.MapControllers();
app.UseAuthentication();
app.Run();
