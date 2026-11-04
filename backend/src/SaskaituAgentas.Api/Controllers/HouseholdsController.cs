using Microsoft.AspNetCore.Mvc;
using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Services;

namespace SaskaituAgentas.Api.Controllers;

[ApiController]
[Route("api/households")]
[Produces("application/json")]
public class HouseholdsController(IHouseholdService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<HouseholdResponse>>> GetAll(CancellationToken ct) =>
        Ok(await service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdResponse>> GetById(Guid id, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) is { } h ? Ok(h) : NotFound();

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<HouseholdResponse>> Create(CreateHouseholdRequest request, CancellationToken ct)
    {
        var result = await service.CreateAsync(request, ct);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : this.ToProblem(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdResponse>> Update(Guid id, UpdateHouseholdRequest request, CancellationToken ct) =>
        await service.UpdateAsync(id, request, ct) is { } h ? Ok(h) : NotFound();

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
