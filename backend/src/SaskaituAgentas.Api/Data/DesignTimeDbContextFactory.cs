using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SaskaituAgentas.Api.Data;

/// <summary>
/// Naudojama tik `dotnet ef` komandoms (migracijų kūrimui), kad nereikėtų paleisti visos aplikacijos.
/// Prisijungimo eilutė imama iš ConnectionStrings__Default aplinkos kintamojo arba vietinio numatytojo.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string LocalDefault =
        "Host=localhost;Port=5432;Database=saskaitu_agentas;Username=saskaitu_agentas;Password=saskaitu_agentas";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__Default") ?? LocalDefault;
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
            .Options;
        return new AppDbContext(options);
    }
}
