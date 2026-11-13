using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SaskaituAgentas.Api.Auth;
using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Services;

namespace SaskaituAgentas.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[EnableRateLimiting(AuthController.RateLimitPolicy)]
public class AuthController(IAuthService service) : ControllerBase
{
    public const string RateLimitPolicy = "auth";

    /// <summary>Registracija: sukuria vartotoją, jo namų ūkį ir grąžina žetonus.</summary>
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await service.RegisterAsync(request, ct);
        return result.IsSuccess ? CreatedAtAction(nameof(Me), null, result.Value) : this.ToProblem(result);
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await service.LoginAsync(request, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result);
    }

    /// <summary>Naujas prieigos žetonas pagal atnaujinimo žetoną. Senasis atnaujinimo žetonas atšaukiamas.</summary>
    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshRequest request, CancellationToken ct)
    {
        var result = await service.RefreshAsync(request.RefreshToken, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result);
    }

    /// <summary>Atsijungimas: atšaukia atnaujinimo žetoną. Prieigos žetonas galioja iki savo pabaigos.</summary>
    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        await service.LogoutAsync(request.RefreshToken, ct);
        return NoContent();
    }

    /// <summary>Išsiunčia slaptažodžio atkūrimo nuorodą. Visada 202 – neatskleidžiama, ar paskyra egzistuoja.</summary>
    [HttpPost("forgot-password")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken ct)
    {
        await service.ForgotPasswordAsync(request.Email, ct);
        return Accepted();
    }

    /// <summary>Nustato naują slaptažodį pagal atkūrimo žetoną ir atjungia visas sesijas.</summary>
    [HttpPost("reset-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken ct)
    {
        var result = await service.ResetPasswordAsync(request, ct);
        return result.IsSuccess ? NoContent() : this.ToProblem(result);
    }

    [HttpGet("me")]
    [Authorize]
    [DisableRateLimiting]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> Me(CancellationToken ct) =>
        await service.GetUserAsync(User.GetUserId(), ct) is { } u ? Ok(u) : Unauthorized();
}
