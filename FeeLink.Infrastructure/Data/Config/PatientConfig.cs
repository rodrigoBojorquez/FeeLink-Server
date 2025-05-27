using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class PatientConfig : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasMany(p => p.TherapistAssignments)
            .WithOne(ta => ta.Patient)
            .HasForeignKey(ta => ta.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(p => p.TutorAssignments)
            .WithOne(tu => tu.Patient)
            .HasForeignKey(tu => tu.PatientId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}