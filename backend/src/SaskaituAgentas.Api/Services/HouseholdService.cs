using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Repositories;

namespace SaskaituAgentas.Api.Services;

public class HouseholdService(IHouseholdRepository households) : IHouseholdService
{
    public async Task<HouseholdResponse?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await households.GetByIdAsync(id, ct) is { } h ? HouseholdResponse.From(h) : null;

    public async Task<HouseholdResponse?> UpdateAsync(Guid id, UpdateHouseholdRequest request, CancellationToken ct = default)
    {
        var household = await households.GetByIdAsync(id, ct);
        if (household is null) return null;

        household.Name = request.Name.Trim();
        await households.SaveChangesAsync(ct);
        return HouseholdResponse.From(household);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var household = await households.GetByIdAsync(id, ct);
        if (household is null) return false;

        households.Remove(household);
        await households.SaveChangesAsync(ct);
        return true;
    }
}
