using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaskaituAgentas.Api.Auth;
using SaskaituAgentas.Api.Configuration;
using SaskaituAgentas.Api.Controllers;
using SaskaituAgentas.Api.Data;
using SaskaituAgentas.Api.Models;
using SaskaituAgentas.Api.Repositories;
using SaskaituAgentas.Api.Services;

// .env → aplinkos kintamieji (turi būti prieš CreateBuilder, kad pakliūtų į konfigūraciją).
var dotEnvPath = DotEnv.Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Nenurodyta ConnectionStrings:Default (pvz. .env faile: ConnectionStrings__Default=...).");

builder.Services.AddDbContext<AppDbContext>(options => options
    .UseNpgsql(connectionString)
    .UseSnakeCaseNamingConvention());

// Nustatymai (validuojami paleidžiant – klaidinga konfigūracija neleidžia startuoti)
builder.Services.AddOptions<JwtSettings>().BindConfiguration(JwtSettings.Section).ValidateDataAnnotations().ValidateOnStart();
builder.Services.AddOptions<PasswordResetSettings>().BindConfiguration(PasswordResetSettings.Section).ValidateDataAnnotations().ValidateOnStart();
var corsSettings = builder.Configuration.GetSection(CorsSettings.Section).Get<CorsSettings>() ?? new CorsSettings();

// Repositories
builder.Services.AddScoped<IHouseholdRepository, HouseholdRepository>();
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddSingleton<IEmailSender, LoggingEmailSender>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHouseholdService, HouseholdService>();
builder.Services.AddScoped<IContractService, ContractService>();

// Autentifikacija (JWT Bearer)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((options, jwt) =>
    {
        options.MapInboundClaims = false; // paliekame „sub“, „email“ ir t. t. kaip yra
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwt.Value.Issuer,
            ValidAudience = jwt.Value.Audience,
            IssuerSigningKey = TokenService.CreateSigningKey(jwt.Value),
            NameClaimType = "name",
            RoleClaimType = AppClaims.Role,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });
builder.Services.AddAuthorization();

// Užklausų ribojimas autentifikacijos maršrutams (apsauga nuo slaptažodžių spėliojimo)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy(AuthController.RateLimitPolicy, http => RateLimitPartition.GetFixedWindowLimiter(
        http.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = builder.Configuration.GetValue("RateLimiting:AuthPermitsPerMinute", 20),
            Window = TimeSpan.FromMinutes(1),
        }));
});

// CORS – frontend'as kitoje kilmėje (Next.js)
builder.Services.AddCors(options => options.AddPolicy(CorsSettings.FrontendPolicy, policy => policy
    .WithOrigins(corsSettings.GetOrigins())
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithExposedHeaders("Location")));

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>());

// Sveikatos patikros: /health/live – procesas gyvas; /health/ready – pasiekiama DB
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>("database", tags: [HealthChecks.ReadyTag]);

var app = builder.Build();

if (dotEnvPath is not null)
    app.Logger.LogInformation("Konfigūracija papildyta iš {DotEnvPath}", dotEnvPath);

if (app.Environment.IsDevelopment() && app.Configuration.GetValue<bool>("Database:MigrateOnStartup"))
{
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
}

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.MapOpenApi(); // /openapi/v1.json
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/openapi/v1.json", "SąskaitųAgentas API v1");
        o.RoutePrefix = "swagger";
        o.DocumentTitle = "SąskaitųAgentas API";
        o.EnablePersistAuthorization();
    });
}

app.UseCors(CorsSettings.FrontendPolicy);
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks();

app.Run();

public partial class Program;
