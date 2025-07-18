namespace FeeLink.Application.UseCases.Roles.Common;

public record RoleResult(
    Guid Id, string Name, string DisplayName, string? Description);