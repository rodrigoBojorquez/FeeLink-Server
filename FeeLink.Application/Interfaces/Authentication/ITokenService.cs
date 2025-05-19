using ErrorOr;
using FeeLink.Application.UseCases.Authentication.Common;

namespace FeeLink.Application.Interfaces.Authentication;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(Domain.Entities.User user);
    string GenerateRefreshToken();
    Task<bool> ValidateRefreshTokenAsync(string refreshToken);
    Task StoreRefreshTokenAsync(string refreshToken, Guid userId);
    Task<ErrorOr<AuthResult>> RefreshToken(string refreshToken);
    Task DeleteRefreshTokenAsync(string refreshToken);
    Task<string?> GetRefreshTokenAsync(Guid userId);
}