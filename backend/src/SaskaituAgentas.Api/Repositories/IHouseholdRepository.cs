using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Repositories;

public interface IHouseholdRepository
{
    Task<IReadOnlyList<Household>> GetAllAsync(CancellationToken ct = default);
    Task<Household?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task AddAsync(Household household, CancellationToken ct = default);
    void Remove(Household household);
    Task SaveChangesAsync(CancellationToken ct = default);
}
