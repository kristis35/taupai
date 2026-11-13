namespace SaskaituAgentas.Api.Models;

/// <summary>Namų ūkis – pagrindinis kliento vienetas. Jam priklauso vartotojai ir sutartys.</summary>
public class Household
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public List<User> Members { get; set; } = [];
    public List<Contract> Contracts { get; set; } = [];
}
