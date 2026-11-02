using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public interface IContractRepository
{
    Task<IReadOnlyList<Contract>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default);
    Task<Contract?> GetByIdAsync(Guid householdId, Guid contractId, CancellationToken ct = default);
    Task AddAsync(Contract contract, CancellationToken ct = default);
    void Remove(Contract contract);
    Task SaveChangesAsync(CancellationToken ct = default);
}
