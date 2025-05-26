using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class PatientConfig : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasOne<Toy>().WithOne().OnDelete(DeleteBehavior.Cascade);
    }
}