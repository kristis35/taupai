using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SaskaituAgentas.Api.Models;

namespace SaskaituAgentas.Api.Data.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Category).HasConversion<string>().HasMaxLength(32);
        builder.Property(c => c.ProviderName).HasMaxLength(200).IsRequired();
        builder.Property(c => c.PlanName).HasMaxLength(200);
        builder.Property(c => c.MonthlyCost).HasPrecision(10, 2);

        builder.HasIndex(c => c.HouseholdId);
        builder.HasIndex(c => c.EndDate);
    }
}
