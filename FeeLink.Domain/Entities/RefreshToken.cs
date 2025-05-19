namespace FeeLink.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Token { get; set; }
    public DateTime Expires { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}