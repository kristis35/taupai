using SaskaituAgentas.Api.Dtos;

namespace SaskaituAgentas.Api.Services;

public interface IHouseholdService
{
    Task<IReadOnlyList<HouseholdResponse>> GetAllAsync(CancellationToken ct = default);
    Task<HouseholdResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ServiceResult<HouseholdResponse>> CreateAsync(CreateHouseholdRequest request, CancellationToken ct = default);
    Task<HouseholdResponse?> UpdateAsync(Guid id, UpdateHouseholdRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
