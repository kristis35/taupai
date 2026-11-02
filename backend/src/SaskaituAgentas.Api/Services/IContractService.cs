using SaskaituAgentas.Api.Dtos;

namespace SaskaituAgentas.Api.Services;

public interface IContractService
{
    Task<ServiceResult<IReadOnlyList<ContractResponse>>> GetByHouseholdAsync(Guid householdId, CancellationToken ct = default);
    Task<ContractResponse?> GetByIdAsync(Guid householdId, Guid contractId, CancellationToken ct = default);
    Task<ServiceResult<ContractResponse>> CreateAsync(Guid householdId, UpsertContractRequest request, CancellationToken ct = default);
    Task<ServiceResult<ContractResponse>> UpdateAsync(Guid householdId, Guid contractId, UpsertContractRequest request, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid householdId, Guid contractId, CancellationToken ct = default);
}
