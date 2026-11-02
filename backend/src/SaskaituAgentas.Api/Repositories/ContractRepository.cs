using Microsoft.EntityFrameworkCore;
using SaskaituAgentas.Api.Data;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public class ContractRepository(AppDbContext db) : IContractRepository
{
    public async Task<IReadOnlyList<Contract>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default) =>
        await db.Contracts.AsNoTracking()
            .Where(c => c.HouseholdId == householdId)
            .OrderBy(c => c.Category).ThenBy(c => c.ProviderName)
            .ToListAsync(ct);

    public Task<Contract?> GetByIdAsync(Guid householdId, Guid contractId, CancellationToken ct = default) =>
        db.Contracts.FirstOrDefaultAsync(c => c.HouseholdId == householdId && c.Id == contractId, ct);

    public async Task AddAsync(Contract contract, CancellationToken ct = default) =>
        await db.Contracts.AddAsync(contract, ct);

    public void Remove(Contract contract) => db.Contracts.Remove(contract);

    public Task SaveChangesAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
