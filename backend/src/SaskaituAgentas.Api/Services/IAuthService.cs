using SaskaituAgentas.Api.Dtos;

namespace SaskaituAgentas.Api.Services;

public interface IAuthService
{
    /// <summary>Sukuria vartotoją ir jo namų ūkį (vartotojas tampa savininku) ir iškart prijungia.</summary>
    Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>Pakeičia galiojantį atnaujinimo žetoną nauju (rotacija).</summary>
    Task<ServiceResult<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>Atšaukia atnaujinimo žetoną. Idempotentiška – nežinomas žetonas nėra klaida.</summary>
    Task LogoutAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>Išsiunčia atkūrimo nuorodą, jei vartotojas egzistuoja. Rezultatas neatskleidžia, ar egzistuoja.</summary>
    Task ForgotPasswordAsync(string email, CancellationToken ct = default);
    Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);

    Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken ct = default);
}
