namespace SaskaituAgentas.Api.Models;

/// <summary>Namų ūkis – pagrindinis kliento vienetas.</summary>
public class Household
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<Contract> Contracts { get; set; } = [];
}
