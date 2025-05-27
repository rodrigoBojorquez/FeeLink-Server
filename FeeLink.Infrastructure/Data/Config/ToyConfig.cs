using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class ToyConfig : IEntityTypeConfiguration<Toy>
{
    public void Configure(EntityTypeBuilder<Toy> builder)
    {
        builder.HasKey(t => t.Id);
        
        builder.HasOne(t => t.Patient)
            .WithOne(p => p.Toy)
            .HasForeignKey<Toy>(t => t.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}