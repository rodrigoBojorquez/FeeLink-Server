using FeeLink.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeeLink.Infrastructure.Data.Config;

public class TherapistAssignmentConfig : IEntityTypeConfiguration<TherapistAssignment>
{
    public void Configure(EntityTypeBuilder<TherapistAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(x => x.User)
            .WithMany(u => u.TherapistAssignments)
            .HasForeignKey(x => x.UserId);
        builder.HasOne(x => x.Patient)
            .WithMany(p => p.TherapistAssignments)
            .HasForeignKey(x => x.PatientId);
    }
}