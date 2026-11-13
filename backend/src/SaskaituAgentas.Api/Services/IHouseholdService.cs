using SaskaituAgentas.Api.Dtos;

namespace SaskaituAgentas.Api.Services;

/// <summary>Namų ūkiai kuriami registracijos metu (<see cref="IAuthService"/>), todėl čia kūrimo nėra.</summary>
public interface IHouseholdService
{
    Task<HouseholdResponse?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<HouseholdResponse?> UpdateAsync(Guid id, UpdateHouseholdRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
