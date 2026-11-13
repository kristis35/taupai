using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaskaituAgentas.Api.Auth;
using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Services;

namespace SaskaituAgentas.Api.Controllers;

[ApiController]
[Authorize]
[HouseholdAccess("householdId")]
[Route("api/households/{householdId:guid}/contracts")]
[Produces("application/json")]
public class ContractsController(IContractService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<ContractResponse>>> GetAll(Guid householdId, CancellationToken ct)
    {
        var result = await service.GetByHouseholdAsync(householdId, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result);
    }

    [HttpGet("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractResponse>> GetById(Guid householdId, Guid contractId, CancellationToken ct) =>
        await service.GetByIdAsync(householdId, contractId, ct) is { } c ? Ok(c) : NotFound();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractResponse>> Create(Guid householdId, UpsertContractRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(householdId, request, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { householdId, contractId = result.Value!.Id }, result.Value)
            : this.ToProblem(result);
    }

    [HttpPut("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContractResponse>> Update(Guid householdId, Guid contractId, UpsertContractRequest request, CancellationToken ct)
    {
        var result = await service.UpdateAsync(householdId, contractId, request, ct);
        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result);
    }

    [HttpDelete("{contractId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid householdId, Guid contractId, CancellationToken ct) =>
        await service.DeleteAsync(householdId, contractId, ct) ? NoContent() : NotFound();
}
