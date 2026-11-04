using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SaskaituAgentas.Api.Data;
using SaskaituAgentas.Api.Repositories;
using SaskaituAgentas.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Nenurodyta ConnectionStrings:Default.");

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(connectionString)
    .UseSnakeCaseNamingConvention());

// Repositories
builder.Services.AddScoped<IHouseholdRepository, HouseholdRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();

// Services
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<IContractService, ContractService>();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    if (app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
    {
        using var scope = app.Services.CreateScope();
        await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
    }
}

app.UseExceptionHandler();
app.UseStatusCodePages();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

public partial class Program;
