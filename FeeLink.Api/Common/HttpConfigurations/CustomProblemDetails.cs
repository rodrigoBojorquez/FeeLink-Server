using ErrorOr;

namespace FeeLink.Api.Common.HttpConfigurations;

public static class CustomProblemDetails
{
    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        
        services.AddProblemDetails(config =>
        {
            config.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                var errors = context.HttpContext.Items["Errors"] as List<Error>;

                if (errors is not null)
                    context.ProblemDetails.Extensions.Add("errorCodes", errors.Select(e => e.Code));
            };
        });

        return services;
    }
}