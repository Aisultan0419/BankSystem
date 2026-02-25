using BankSystemAPI.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiBasics(builder.Configuration);
builder.Services.AddApiLogging(builder.Configuration);
builder.Host.UseApiLogging();
builder.Services.AddRabbitMq();
builder.Services.AddApiAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddPanEncryptor(builder.Configuration);
var app = builder.Build();

app.UseStaticFiles();

app.UseApiSwagger();
app.UseMiddlewares();
app.UseGlobalExceptionHandler();
app.MapControllers();

await app.ApplyDatabaseMigrations();

app.MapGet("/", ctx => { ctx.Response.Redirect("/swagger"); return Task.CompletedTask; });


app.Run();
