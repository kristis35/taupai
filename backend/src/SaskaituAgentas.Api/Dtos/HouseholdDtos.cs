using System.ComponentModel.DataAnnotations;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Dtos;

public record HouseholdResponse(Guid Id, string Name, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static HouseholdResponse From(Household h) => new(h.Id, h.Name, h.CreatedAt, h.UpdatedAt);
}

public record UpdateHouseholdRequest(
    [Required, StringLength(200, MinimumLength = 1)] string Name);
