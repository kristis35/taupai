using System.ComponentModel.DataAnnotations;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Dtos;

public record HouseholdResponse(Guid Id, string Name, string Email, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt)
{
    public static HouseholdResponse From(Household h) => new(h.Id, h.Name, h.Email, h.CreatedAt, h.UpdatedAt);
}

public record CreateHouseholdRequest(
    [Required, StringLength(200, MinimumLength = 1)] string Name,
    [Required, EmailAddress, StringLength(320)] string Email);

public record UpdateHouseholdRequest(
    [Required, StringLength(200, MinimumLength = 1)] string Name);
