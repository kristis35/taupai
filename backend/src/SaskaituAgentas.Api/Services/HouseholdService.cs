using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Models;
using SaskaituAgentas.Api.Repositories;

namespace SaskaituAgentas.Api.Services;

public class HouseholdService(IHouseholdRepository households) : IHouseholdService
{
    public async Task<IReadOnlyList<HouseholdResponse>> GetAllAsync(CancellationToken ct = default) =>
        (await households.GetAllAsync(ct)).Select(HouseholdResponse.From).ToList();

    public async Task<HouseholdResponse?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await households.GetByIdAsync(id, ct) is { } h ? HouseholdResponse.From(h) : null;

    public async Task<ServiceResult<HouseholdResponse>> CreateAsync(CreateHouseholdRequest request, CancellationToken ct = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        if (await households.EmailExistsAsync(email, ct))
            return ServiceResult<HouseholdResponse>.Fail(ServiceError.Conflict, "Namų ūkis su šiuo el. paštu jau egzistuoja.");

        var household = new Household { Id = Guid.CreateVersion7(), Name = request.Name.Trim(), Email = email };
        await households.AddAsync(household, ct);
        await households.SaveChangesAsync(ct);
        return ServiceResult<HouseholdResponse>.Ok(HouseholdResponse.From(household));
    }

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
