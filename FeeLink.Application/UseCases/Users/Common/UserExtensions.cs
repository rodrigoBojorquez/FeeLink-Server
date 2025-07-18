using FeeLink.Domain.Entities;

namespace FeeLink.Application.UseCases.Users.Common;

public static class UserExtensions
{
    public static UserResult ToResult(this User user)
    {
        return new UserResult(user.Id,
            user.Name,
            user.Picture,
            user.Email,
            user.Role?.Name ?? string.Empty,
            user.Role?.DisplayName ?? string.Empty);
    }
}