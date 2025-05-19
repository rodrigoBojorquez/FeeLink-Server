using ErrorOr;
using FeeLink.Application.Common.Results;

namespace FeeLink.Application.Interfaces.Authentication;

public interface IAuthService
{
    void SetRefreshToken(string token);
    ErrorOr<Guid> GetUserId();
    bool HasSuperAccess();
}