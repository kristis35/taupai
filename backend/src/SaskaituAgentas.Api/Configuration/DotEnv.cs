namespace SaskaituAgentas.Api.Configuration;

/// <summary>
/// Minimalus <c>.env</c> failo skaitytuvas. Kintamieji įrašomi į proceso aplinką prieš kuriant
/// <see cref="WebApplicationBuilder"/>, todėl jie patenka į konfigūraciją per įprastą
/// aplinkos kintamųjų šaltinį (pvz. <c>Jwt__SigningKey</c> → <c>Jwt:SigningKey</c>).
/// Jau nustatyti aplinkos kintamieji nėra perrašomi – tikra aplinka turi pirmenybę prieš failą.
/// </summary>
public static class DotEnv
{
    /// <summary>
    /// Ieško <c>.env</c> nuo darbinio katalogo aukštyn iki repozitorijos šaknies (katalogo su <c>.git</c>).
    /// Grąžina įkelto failo kelią arba <c>null</c>, jei failas nerastas.
    /// </summary>
    public static string? Load(string? startDirectory = null)
    {
        var dir = new DirectoryInfo(startDirectory ?? Directory.GetCurrentDirectory());
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, ".env");
            if (File.Exists(candidate))
            {
                LoadFile(candidate);
                return candidate;
            }
            if (Directory.Exists(Path.Combine(dir.FullName, ".git"))) break;
            dir = dir.Parent;
        }
        return null;
    }

    public static void LoadFile(string path)
    {
        foreach (var rawLine in File.ReadAllLines(path))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line.StartsWith('#')) continue;
            if (line.StartsWith("export ", StringComparison.Ordinal)) line = line[7..].TrimStart();

            var eq = line.IndexOf('=');
            if (eq <= 0) continue;

            var key = line[..eq].Trim();
            var value = line[(eq + 1)..].Trim();
            if (value.Length >= 2 && (value[0] == '"' && value[^1] == '"' || value[0] == '\'' && value[^1] == '\''))
                value = value[1..^1];
            else if (value.IndexOf(" #", StringComparison.Ordinal) is var comment and >= 0)
                value = value[..comment].TrimEnd();

            if (Environment.GetEnvironmentVariable(key) is null)
                Environment.SetEnvironmentVariable(key, value);
        }
    }
}
