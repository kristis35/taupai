using System.ComponentModel.DataAnnotations;

namespace SaskaituAgentas.Api.Auth;

public class JwtSettings
{
    public const string Section = "Jwt";

    [Required] public string Issuer { get; set; } = "saskaitu-agentas";
    [Required] public string Audience { get; set; } = "saskaitu-agentas";

    /// <summary>HMAC-SHA256 raktas. Bent 32 simboliai; produkcijoje – tik per aplinkos kintamąjį <c>Jwt__SigningKey</c>.</summary>
    [Required, MinLength(32, ErrorMessage = "Jwt:SigningKey turi būti bent 32 simbolių.")]
    public string SigningKey { get; set; } = "";

    [Range(1, 1440)] public int AccessTokenMinutes { get; set; } = 15;
    [Range(1, 365)] public int RefreshTokenDays { get; set; } = 30;
}

public class PasswordResetSettings
{
    public const string Section = "PasswordReset";

    /// <summary>Frontend puslapis, į kurį veda laiške esanti nuoroda; prie jo pridedamas <c>?token=...</c>.</summary>
    [Required, Url] public string ResetUrl { get; set; } = "http://localhost:3000/reset-password";

    [Range(5, 1440)] public int TokenMinutes { get; set; } = 60;
}
