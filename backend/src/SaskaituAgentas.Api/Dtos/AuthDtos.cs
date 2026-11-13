using System.ComponentModel.DataAnnotations;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Dtos;

public record RegisterRequest(
    [Required, EmailAddress, StringLength(320)] string Email,
    [Required, StringLength(128, MinimumLength = 8)] string Password,
    [Required, StringLength(200, MinimumLength = 1)] string FullName,
    /// <summary>Namų ūkio pavadinimas; jei nenurodytas – „{Vardas} namų ūkis“.</summary>
    [StringLength(200)] string? HouseholdName);

public record LoginRequest(
    [Required, EmailAddress] string Email,
    [Required] string Password);

public record RefreshRequest([Required] string RefreshToken);

public record LogoutRequest([Required] string RefreshToken);

public record ForgotPasswordRequest([Required, EmailAddress] string Email);

public record ResetPasswordRequest(
    [Required] string Token,
    [Required, StringLength(128, MinimumLength = 8)] string NewPassword);

public record UserResponse(Guid Id, string Email, string FullName, Guid HouseholdId, HouseholdRole Role, DateTimeOffset CreatedAt)
{
    public static UserResponse From(User u) => new(u.Id, u.Email, u.FullName, u.HouseholdId, u.Role, u.CreatedAt);
}

public record AuthResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    UserResponse User);
