using FeeLink.Api.Common.Attributes;
using FeeLink.Domain.Common.Constants;

namespace FeeLink.Api.Common.Middlewares;

public class RoleAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint is null)
        {
            await next(context);
            return;
        }

        var permissionAttribute = endpoint.Metadata.GetMetadata<RequiredRolesAttribute>();

        if (permissionAttribute is null)
        {
            await next(context);
            return;
        }

        var requiredRoles = permissionAttribute.Roles;
        var user = context.User;

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        var roleClaim = user.FindFirst(c => c.Type == FeeLinkConstants.RoleClaim)?.Value;

        if (!string.IsNullOrEmpty(roleClaim) &&
            !requiredRoles.Contains(FeeLinkConstants.SuperAccessPermission) && requiredRoles.Contains(roleClaim))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await next(context);
    }
}