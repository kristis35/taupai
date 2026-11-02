namespace SaskaituAgentas.Api.Models;

/// <summary>Namų ūkio sutartis su paslaugų teikėju (elektra, mobilusis ryšys ir t. t.).</summary>
public class Contract
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Household? Household { get; set; }

    public ContractCategory Category { get; set; }
    public required string ProviderName { get; set; }
    public string? PlanName { get; set; }
    public decimal MonthlyCost { get; set; }

    public DateOnly? StartDate { get; set; }

    /// <summary>Terminuotos sutarties pabaiga – pagal ją tikrinamos kainos prieš atnaujinimą.</summary>
    public DateOnly? EndDate { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
