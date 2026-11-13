using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);

    /// <summary>Grąžina žetoną kartu su vartotoju.</summary>
    Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default);
    Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default);
    Task RevokeAllRefreshTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct = default);

    /// <summary>Grąžina žetoną kartu su vartotoju.</summary>
    Task<PasswordResetToken?> GetPasswordResetTokenAsync(string tokenHash, CancellationToken ct = default);
    Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken ct = default);
    Task InvalidatePasswordResetTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);
}
