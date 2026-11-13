using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Auth;

public static class AppClaims
{
    public const string HouseholdId = "household_id";
    public const string Role = "role";
}

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? throw new InvalidOperationException("Žetone nėra 'sub' teiginio."));

    public static Guid GetHouseholdId(this ClaimsPrincipal user) =>
        Guid.Parse(user.FindFirstValue(AppClaims.HouseholdId)
            ?? throw new InvalidOperationException("Žetone nėra 'household_id' teiginio."));

    public static bool IsHouseholdOwner(this ClaimsPrincipal user) =>
        user.FindFirstValue(AppClaims.Role) == nameof(HouseholdRole.Owner);
}
