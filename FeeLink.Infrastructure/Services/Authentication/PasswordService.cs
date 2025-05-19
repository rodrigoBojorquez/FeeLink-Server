using System.Text;
using FeeLink.Application.Interfaces.Authentication;

namespace FeeLink.Infrastructure.Services.Authentication;

public class PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    
    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
    
    /**
     * Genera:
     * - 6 caracteres aleatorios
     * - Prefijo fijo "User"
     * - otros 6 caracteres aleatorios
     */
    public string GeneratePassword(string email)
    {
        var randomString = Guid.NewGuid().ToString("N").Substring(0, 6);
        var password = new StringBuilder();

        password.Append("User");
        password.Append(randomString);

        return password.ToString();
    }
}