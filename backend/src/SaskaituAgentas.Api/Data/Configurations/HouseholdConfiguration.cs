using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Data.Configurations;

public class HouseholdConfiguration : IEntityTypeConfiguration<Household>
{
    public void Configure(EntityTypeBuilder<Household> builder)
    {
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Name).HasMaxLength(200).IsRequired();
        builder.Property(h => h.Email).HasMaxLength(320).IsRequired();
        builder.HasIndex(h => h.Email).IsUnique();

        builder.HasMany(h => h.Contracts)
            .WithOne(c => c.Household)
            .HasForeignKey(c => c.HouseholdId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
