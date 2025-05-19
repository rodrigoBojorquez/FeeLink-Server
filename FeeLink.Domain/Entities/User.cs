namespace FeeLink.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Email { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public string? Password { get; set; }
    public string? RecoveryToken { get; set; }
    public string? GoogleId { get; set; }
    public string? Picture { get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; } = null!;
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;
    public DateTime? UpdateDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeleteDate { get; set; }
    
    public Guid? CompanyId { get; set; }
    public Company Company { get; set; } = null!;

    public List<Patient> Patients { get; set; } = [];
}