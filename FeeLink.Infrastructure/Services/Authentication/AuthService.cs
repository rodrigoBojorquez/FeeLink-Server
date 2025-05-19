using System.Security.Authentication;
using System.Security.Claims;
using ErrorOr;
using FeeLink.Application.Common.Results;
using FeeLink.Application.Interfaces.Authentication;
using FeeLink.Application.Interfaces.Repositories;
using FeeLink.Domain.Common.Constants;
using FeeLink.Domain.Common.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FeeLink.Infrastructure.Services.Authentication;

public class AuthService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    : IAuthService
{
    public void SetRefreshToken(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(config.GetValue<int>("Authentication:RefreshTokenExpireDays")),
            SameSite = SameSiteMode.None,
            Secure = true
        };

        httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

    public ErrorOr<Guid> GetUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        var claim = user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(claim, out var userId)
            ? userId
            : Errors.Authentication.NotAuthenticated;
    }

    public bool HasSuperAccess()
    {
        var claim = httpContextAccessor.HttpContext?.User?
            .Claims.FirstOrDefault(c => c.Type == FeeLinkConstants.RoleClaim)?.Value;

        return !string.IsNullOrEmpty(claim) && claim == FeeLinkConstants.AdminRole;
    }
}