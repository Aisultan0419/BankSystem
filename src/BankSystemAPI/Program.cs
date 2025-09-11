using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Mappings;
using Application.Services;
using BankSystem;
using FluentValidation.AspNetCore;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Application.Interfaces.Auth;
using Domain.Configuration;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRegisterService, UserRegisterService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host = localhost; Port = 5432; Database = BankSystem; Username = postgres; Password = 210624"));
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
var app = builder.Build();
if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "My Bank API");
        });
    }
app.UseAuthorization();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.Run();
