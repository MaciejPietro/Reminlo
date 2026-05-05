// /src/Reminlo.Application/DependencyInjection.cs

using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Reminlo.Application.Config;

namespace Reminlo.Application;

/// <summary>
/// Provides extension methods for registering application-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers application services, MediatR handlers, and Mapster configuration.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddMapster();
        MapsterConfig.Configure();

        return services;
    }
}
