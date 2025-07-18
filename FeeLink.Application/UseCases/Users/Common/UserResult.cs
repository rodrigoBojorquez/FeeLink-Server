namespace FeeLink.Application.UseCases.Users.Common;

public record UserResult(
    Guid Id,
    string Name,
    string? Picture,
    string Email,
    string RoleName,
    string RoleDisplayName);