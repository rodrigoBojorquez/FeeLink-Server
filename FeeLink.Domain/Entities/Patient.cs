namespace FeeLink.Domain.Entities;

public class Patient
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public float Height { get; set; }
    public float Weight { get; set; }
    
    public Toy Toy { get; set; } = null!;

    public List<TherapistAssignment> TherapistAssignments { get; set; } = [];
    public List<TutorAssignment> TutorAssignments { get; set; } = [];
}