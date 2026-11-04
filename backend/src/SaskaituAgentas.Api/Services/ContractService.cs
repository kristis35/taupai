using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Models;
using SaskaituAgentas.Api.Repositories;

namespace SaskaituAgentas.Api.Services;

public class ContractService(IContractRepository contracts, IHouseholdRepository households) : IContractService
{
    private const string HouseholdNotFound = "Namų ūkis nerastas.";

    public async Task<ServiceResult<IReadOnlyList<ContractResponse>>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default)
    {
        if (!await households.ExistsAsync(householdId, ct))
            return ServiceResult<IReadOnlyList<ContractResponse>>.Fail(ServiceError.NotFound, HouseholdNotFound);

        var list = (await contracts.GetByHouseholdAsync(householdId, ct)).Select(ContractResponse.From).ToList();
        return ServiceResult<IReadOnlyList<ContractResponse>>.Ok(list);
    }

    public async Task<ContractResponse?> GetByIdAsync(Guid householdId, Guid contractId, CancellationToken ct = default) =>
        await contracts.GetByIdAsync(householdId, contractId, ct) is { } c ? ContractResponse.From(c) : null;

    public async Task<ServiceResult<ContractResponse>> CreateAsync(Guid householdId, UpsertContractRequest request, CancellationToken ct = default)
    {
        if (Validate(request) is { } error)
            return ServiceResult<ContractResponse>.Fail(ServiceError.Validation, error);
        if (!await households.ExistsAsync(householdId, ct))
            return ServiceResult<ContractResponse>.Fail(ServiceError.NotFound, HouseholdNotFound);

        var contract = new Contract { Id = Guid.CreateVersion7(), HouseholdId = householdId, ProviderName = "" };
        Apply(contract, request);
        await contracts.AddAsync(contract, ct);
        await contracts.SaveChangesAsync(ct);
        return ServiceResult<ContractResponse>.Ok(ContractResponse.From(contract));
    }

    public async Task<ServiceResult<ContractResponse>> UpdateAsync(Guid householdId, Guid contractId, UpsertContractRequest request, CancellationToken ct = default)
    {
        if (Validate(request) is { } error)
            return ServiceResult<ContractResponse>.Fail(ServiceError.Validation, error);

        var contract = await contracts.GetByIdAsync(householdId, contractId, ct);
        if (contract is null)
            return ServiceResult<ContractResponse>.Fail(ServiceError.NotFound, "Sutartis nerasta.");

        Apply(contract, request);
        await contracts.SaveChangesAsync(ct);
        return ServiceResult<ContractResponse>.Ok(ContractResponse.From(contract));
    }

    public async Task<bool> DeleteAsync(Guid householdId, Guid contractId, CancellationToken ct = default)
    {
        var contract = await contracts.GetByIdAsync(householdId, contractId, ct);
        if (contract is null) return false;

        contracts.Remove(contract);
        await contracts.SaveChangesAsync(ct);
        return true;
    }

    private static string? Validate(UpsertContractRequest r) =>
        r.StartDate is { } start && r.EndDate is { } end && end < start
            ? "Sutarties pabaigos data negali būti ankstesnė už pradžios datą."
            : null;

    private static void Apply(Contract c, UpsertContractRequest r)
    {
        c.Category = r.Category;
        c.ProviderName = r.ProviderName.Trim();
        c.PlanName = string.IsNullOrWhiteSpace(r.PlanName) ? null : r.PlanName.Trim();
        c.MonthlyCost = r.MonthlyCost;
        c.StartDate = r.StartDate;
        c.EndDate = r.EndDate;
    }
}
