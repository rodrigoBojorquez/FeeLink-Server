using FeeLink.Domain.Entities;

namespace FeeLink.Application.Interfaces.Authentication;

public interface IJwtGenerator
{
    string GenerateToken(User user);
}