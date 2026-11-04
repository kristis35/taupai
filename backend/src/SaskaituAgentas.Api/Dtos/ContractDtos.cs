using System.ComponentModel.DataAnnotations;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Dtos;

public record ContractResponse(
    Guid Id,
    Guid HouseholdId,
    ContractCategory Category,
    string ProviderName,
    string? PlanName,
    decimal MonthlyCost,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt)
{
    public static ContractResponse From(Contract c) => new(
        c.Id, c.HouseholdId, c.Category, c.ProviderName, c.PlanName,
        c.MonthlyCost, c.StartDate, c.EndDate, c.CreatedAt, c.UpdatedAt);
}

public record UpsertContractRequest(
    [Required, EnumDataType(typeof(ContractCategory))] ContractCategory Category,
    [Required, StringLength(200, MinimumLength = 1)] string ProviderName,
    [StringLength(200)] string? PlanName,
    [Range(0, 100000)] decimal MonthlyCost,
    DateOnly? StartDate,
    DateOnly? EndDate);
