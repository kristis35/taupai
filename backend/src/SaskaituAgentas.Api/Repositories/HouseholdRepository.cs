using Microsoft.EntityFrameworkCore;
using SaskaituAgentas.Api.Data;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public class HouseholdRepository(AppDbContext db) : IHouseholdRepository
{
    public async Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct = default) =>
        await db.Households.AsNoTracking().OrderBy(h => h.CreatedAt).ToListAsync(ct);

    public Task<Household?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Households.FirstOrDefaultAsync(h => h.Id == id, ct);

    public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
        db.Households.AnyAsync(h => h.Id == id, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        db.Households.AnyAsync(h => h.Email == email, ct);

    public async Task AddAsync(Household household, CancellationToken ct = default) =>
        await db.Households.AddAsync(household, ct);

    public void Remove(Household household) => db.Households.Remove(household);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
