namespace FeeLink.Application.UseCases.Authentication.Common;

public record AuthResult(
    Guid Id,
    string AccessToken,
    string RefreshToken,
    string? Email = "",
    bool GoogleAuth = false,
    string? Name = "",
    string? LastName = null,
    string? Picture = null);