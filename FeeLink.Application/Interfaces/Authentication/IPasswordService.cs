namespace FeeLink.Application.Interfaces.Authentication;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    string GeneratePassword(string email);
}