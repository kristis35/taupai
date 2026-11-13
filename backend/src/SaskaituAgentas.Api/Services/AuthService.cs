using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using SaskaituAgentas.Api.Auth;
using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Models;
using SaskaituAgentas.Api.Repositories;

namespace SaskaituAgentas.Api.Services;

public class AuthService(
    IUserRepository users,
    IHouseholdRepository households,
    ITokenService tokens,
    IPasswordHasher<User> hasher,
    IEmailSender email,
    IOptions<JwtSettings> jwtOptions,
    IOptions<PasswordResetSettings> resetOptions,
    TimeProvider clock,
    ILogger<AuthService> logger) : IAuthService
{
    private const string InvalidCredentials = "Neteisingas el. paštas arba slaptažodis.";
    private const string InvalidRefreshToken = "Sesija nebegalioja. Prisijunkite iš naujo.";
    private const string EmailTaken = "Vartotojas su šiuo el. paštu jau egzistuoja.";

    // Naudojamas, kai vartotojas nerastas, kad atsakymo laikas neišduotų, ar el. paštas registruotas.
    private static readonly Lazy<string> DummyHash =
        new(() => new PasswordHasher<User>().HashPassword(null!, Guid.NewGuid().ToString()));

    public async Task<ServiceResult<AuthResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var normalizedEmail = Normalize(request.Email);
        if (await users.EmailExistsAsync(normalizedEmail, ct))
            return ServiceResult<AuthResponse>.Fail(ServiceError.Conflict, EmailTaken);

        var fullName = request.FullName.Trim();
        var household = new Household
        {
            Id = Guid.CreateVersion7(),
            Name = string.IsNullOrWhiteSpace(request.HouseholdName) ? $"{fullName} namų ūkis" : request.HouseholdName.Trim(),
        };
        var user = new User
        {
            Id = Guid.CreateVersion7(),
            Email = normalizedEmail,
            FullName = fullName,
            PasswordHash = "",
            HouseholdId = household.Id,
            Role = HouseholdRole.Owner,
            LastLoginAt = clock.GetUtcNow(),
        };
        user.PasswordHash = hasher.HashPassword(user, request.Password);

        await households.AddAsync(household, ct);
        await users.AddAsync(user, ct);
        var response = await IssueTokensAsync(user, ct);

        try
        {
            await users.SaveChangesAsync(ct); // namų ūkis, vartotojas ir žetonas – viena transakcija
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            return ServiceResult<AuthResponse>.Fail(ServiceError.Conflict, EmailTaken);
        }

        return ServiceResult<AuthResponse>.Ok(response);
    }

    public async Task<ServiceResult<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(Normalize(request.Email), ct);
        if (user is null)
        {
            hasher.VerifyHashedPassword(null!, DummyHash.Value, request.Password);
            return ServiceResult<AuthResponse>.Fail(ServiceError.Unauthorized, InvalidCredentials);
        }

        var verification = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
            return ServiceResult<AuthResponse>.Fail(ServiceError.Unauthorized, InvalidCredentials);
        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
            user.PasswordHash = hasher.HashPassword(user, request.Password);

        user.LastLoginAt = clock.GetUtcNow();
        var response = await IssueTokensAsync(user, ct);
        await users.SaveChangesAsync(ct);
        return ServiceResult<AuthResponse>.Ok(response);
    }

    public async Task<ServiceResult<AuthResponse>> RefreshAsync(string refreshToken, CancellationToken ct = default)
    {
        var now = clock.GetUtcNow();
        var stored = await users.GetRefreshTokenAsync(tokens.Hash(refreshToken), ct);
        if (stored?.User is null)
            return ServiceResult<AuthResponse>.Fail(ServiceError.Unauthorized, InvalidRefreshToken);

        if (stored.RevokedAt is not null)
        {
            // Jau panaudotas (rotuotas) žetonas pateiktas dar kartą – galimai pavogtas. Uždarome visas sesijas.
            logger.LogWarning("Pakartotinai panaudotas atšauktas atnaujinimo žetonas, vartotojas {UserId}", stored.UserId);
            await users.RevokeAllRefreshTokensAsync(stored.UserId, now, ct);
            return ServiceResult<AuthResponse>.Fail(ServiceError.Unauthorized, InvalidRefreshToken);
        }
        if (!stored.IsActive(now))
            return ServiceResult<AuthResponse>.Fail(ServiceError.Unauthorized, InvalidRefreshToken);

        stored.RevokedAt = now;
        var response = await IssueTokensAsync(stored.User, ct);
        await users.SaveChangesAsync(ct);
        return ServiceResult<AuthResponse>.Ok(response);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct = default)
    {
        var stored = await users.GetRefreshTokenAsync(tokens.Hash(refreshToken), ct);
        if (stored is null || stored.RevokedAt is not null) return;

        stored.RevokedAt = clock.GetUtcNow();
        await users.SaveChangesAsync(ct);
    }

    public async Task ForgotPasswordAsync(string emailAddress, CancellationToken ct = default)
    {
        var user = await users.GetByEmailAsync(Normalize(emailAddress), ct);
        if (user is null) return;

        var now = clock.GetUtcNow();
        var settings = resetOptions.Value;
        await users.InvalidatePasswordResetTokensAsync(user.Id, now, ct); // galioja tik naujausia nuoroda

        var (token, hash) = tokens.CreateOpaqueToken();
        await users.AddPasswordResetTokenAsync(new PasswordResetToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAt = now.AddMinutes(settings.TokenMinutes),
        }, ct);
        await users.SaveChangesAsync(ct);

        var link = $"{settings.ResetUrl}?token={Uri.EscapeDataString(token)}";
        await email.SendAsync(
            user.Email,
            "Slaptažodžio atkūrimas – SąskaitųAgentas.lt",
            $"""
            Sveiki, {user.FullName},

            gavome prašymą atkurti jūsų slaptažodį. Naują slaptažodį galite nustatyti čia:
            {link}

            Nuoroda galioja {settings.TokenMinutes} min. Jei to neprašėte, šį laišką tiesiog ignoruokite.
            """,
            ct);
    }

    public async Task<ServiceResult<bool>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default)
    {
        var now = clock.GetUtcNow();
        var stored = await users.GetPasswordResetTokenAsync(tokens.Hash(request.Token), ct);
        if (stored?.User is null || !stored.IsUsable(now))
            return ServiceResult<bool>.Fail(ServiceError.Validation, "Atkūrimo nuoroda neteisinga arba nebegalioja.");

        stored.UsedAt = now;
        stored.User.PasswordHash = hasher.HashPassword(stored.User, request.NewPassword);
        await users.SaveChangesAsync(ct);
        await users.RevokeAllRefreshTokensAsync(stored.UserId, now, ct); // atsijungti visuose įrenginiuose
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<UserResponse?> GetUserAsync(Guid userId, CancellationToken ct = default) =>
        await users.GetByIdAsync(userId, ct) is { } u ? UserResponse.From(u) : null;

    private async Task<AuthResponse> IssueTokensAsync(User user, CancellationToken ct)
    {
        var (accessToken, accessExpires) = tokens.CreateAccessToken(user);
        var (refreshToken, refreshHash) = tokens.CreateOpaqueToken();
        var refreshExpires = clock.GetUtcNow().AddDays(jwtOptions.Value.RefreshTokenDays);

        await users.AddRefreshTokenAsync(new RefreshToken
        {
            Id = Guid.CreateVersion7(),
            UserId = user.Id,
            TokenHash = refreshHash,
            ExpiresAt = refreshExpires,
        }, ct);

        return new AuthResponse(accessToken, accessExpires, refreshToken, refreshExpires, UserResponse.From(user));
    }

    private static string Normalize(string emailAddress) => emailAddress.Trim().ToLowerInvariant();
}
