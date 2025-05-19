namespace FeeLink.Domain.Entities;

public class Role
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    
}