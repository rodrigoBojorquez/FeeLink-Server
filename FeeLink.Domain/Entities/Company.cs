namespace FeeLink.Domain.Entities;

public class Company
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; } = string.Empty;
    public string Rfc { get; set; } = string.Empty; 
    public string PersonContact { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}