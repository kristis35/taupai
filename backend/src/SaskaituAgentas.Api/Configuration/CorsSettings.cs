namespace SaskaituAgentas.Api.Configuration;

public class CorsSettings
{
    public const string Section = "Cors";
    public const string FrontendPolicy = "Frontend";

    /// <summary>Leidžiamos kilmės, atskirtos kableliais (pvz. <c>http://localhost:3000,https://saskaituagentas.lt</c>).</summary>
    public string AllowedOrigins { get; set; } = "";

    public string[] GetOrigins() =>
        AllowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(o => o.TrimEnd('/'))
            .ToArray();
}
