namespace SaskaituAgentas.Api.Models;

/// <summary>Prisijungiantis asmuo. Kiekvienas vartotojas priklauso vienam namų ūkiui.</summary>
public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string FullName { get; set; }
    public required string PasswordHash { get; set; }

    public Guid HouseholdId { get; set; }
    public Household? Household { get; set; }
    public HouseholdRole Role { get; set; } = HouseholdRole.Owner;

    public DateTimeOffset? LastLoginAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public List<PasswordResetToken> PasswordResetTokens { get; set; } = [];
}
