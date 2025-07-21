using ErrorOr;
using FeeLink.Api.HostedServices;
using FeeLink.Application.Common.Errors;
using Microsoft.AspNetCore.Diagnostics;

namespace FeeLink.Api.Common.DependencyInjection;

public static class ServiceContainer
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails(opt =>
        {
            opt.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
                var errors = context.HttpContext.Items["errors"] as List<Error>;

                if (errors is not null)
                    context.ProblemDetails.Extensions.Add("errorCodes", errors.Select(e => e.Code));
            };
        });

        services.AddHttpContextAccessor();
        services.AddHostedService<SensorDataHostedService>();

        return services;
    }

    public static WebApplication UseGlobalErrorHandling(this WebApplication app)
    {
        app.UseExceptionHandler("/error");

        app.Map("/error", (HttpContext httpContext,
            IHostEnvironment env) =>
        {
            Exception? exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            if (exception is null)
                return Results.Problem();

            return exception switch
            {
                IServiceException serviceException => Results.Problem(detail: serviceException.ErrorMessage,
                    statusCode: serviceException.StatusCode),
                _ => Results.Problem(detail: "\ud83d\udd34 Error del servidor, intente más tarde",
                    statusCode: StatusCodes.Status500InternalServerError)
            };
        });

        return app;
    }
}