using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Roles.Common;

public static class RoleExtensions
{
    public static RoleResult ToResult(this Role role) =>
        new RoleResult(role.Id, role.Name, role.DisplayName, role.Description);
}