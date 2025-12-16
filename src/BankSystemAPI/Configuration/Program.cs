using BankSystemAPI.Configuration.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiBasics(builder.Configuration);
builder.Services.AddApiLogging(builder.Configuration);
builder.Host.UseApiLogging();

builder.Services.AddApiAuthentication(builder.Configuration, builder.Environment);

builder.Services.AddPanEncryptor(builder.Configuration);
var app = builder.Build();

app.UseApiSwagger();
app.UseMiddlewares();
app.UseGlobalExceptionHandler();
app.MapControllers();
if (app.Environment.IsEnvironment("Docker"))
{
    await app.ApplyDatabaseMigrations();
}
app.MapGet("/", ctx => { ctx.Response.Redirect("/swagger"); return Task.CompletedTask; });


app.Run();
