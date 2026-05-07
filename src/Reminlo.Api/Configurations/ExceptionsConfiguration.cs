using Reminlo.Api.Middlewares;

namespace Reminlo.Api.Configurations;

using Microsoft.AspNetCore.Builder;


/// <summary>
/// Extension methods for registering the exception handling pipeline.
/// </summary>
public static class ExceptionsConfiguration
{
    
    public static IServiceCollection AddExceptionsHandler(this IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }

    public static IApplicationBuilder UseExceptionsHandler(this IApplicationBuilder app)
    {
        return app.UseExceptionHandler();
    }
}
