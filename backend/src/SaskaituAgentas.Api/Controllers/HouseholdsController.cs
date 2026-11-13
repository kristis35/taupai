using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaskaituAgentas.Api.Auth;
using SaskaituAgentas.Api.Dtos;
using SaskaituAgentas.Api.Services;

namespace SaskaituAgentas.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/households")]
[Produces("application/json")]
public class HouseholdsController(IHouseholdService service) : ControllerBase
{
    /// <summary>Prisijungusio vartotojo namų ūkis.</summary>
    [HttpGet("me")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ActionResult<HouseholdResponse>> GetMine(CancellationToken ct) => GetById(User.GetHouseholdId(), ct);

    [HttpGet("{id:guid}")]
    [HouseholdAccess("id")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdResponse>> GetById(Guid id, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) is { } h ? Ok(h) : NotFound();

    [HttpPut("{id:guid}")]
    [HouseholdAccess("id")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<HouseholdResponse>> Update(Guid id, UpdateHouseholdRequest request, CancellationToken ct) =>
        await service.UpdateAsync(id, request, ct) is { } h ? Ok(h) : NotFound();

    /// <summary>Ištrina namų ūkį kartu su visais jo nariais ir sutartimis. Leidžiama tik savininkui.</summary>
    [HttpDelete("{id:guid}")]
    [HouseholdAccess("id", RequireOwner = true)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        await service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
