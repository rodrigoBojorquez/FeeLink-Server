namespace FeeLink.Application.UseCases.Authentication.Common;

public record AuthResult(
    Guid Id,
    string AccessToken,
    string RefreshToken,
    string? Email = "",
    string? Name = "",
    string? LastName = null,
    string? Picture = null);