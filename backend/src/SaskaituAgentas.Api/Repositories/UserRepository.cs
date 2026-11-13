using Microsoft.EntityFrameworkCore;
using SaskaituAgentas.Api.Data;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken ct = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, ct);

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken ct = default) =>
        db.Users.AnyAsync(u => u.Email == normalizedEmail, ct);

    public async Task AddAsync(User user, CancellationToken ct = default) => await db.Users.AddAsync(user, ct);

    public Task<RefreshToken?> GetRefreshTokenAsync(string tokenHash, CancellationToken ct = default) =>
        db.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddRefreshTokenAsync(RefreshToken token, CancellationToken ct = default) =>
        await db.RefreshTokens.AddAsync(token, ct);

    public Task RevokeAllRefreshTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct = default) =>
        db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.RevokedAt, now), ct);

    public Task<PasswordResetToken?> GetPasswordResetTokenAsync(string tokenHash, CancellationToken ct = default) =>
        db.PasswordResetTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken ct = default) =>
        await db.PasswordResetTokens.AddAsync(token, ct);

    public Task InvalidatePasswordResetTokensAsync(Guid userId, DateTimeOffset now, CancellationToken ct = default) =>
        db.PasswordResetTokens
            .Where(t => t.UserId == userId && t.UsedAt == null)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.UsedAt, now), ct);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
