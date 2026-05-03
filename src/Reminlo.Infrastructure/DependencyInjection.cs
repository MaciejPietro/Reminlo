using Reminlo.Application.Repositories;
using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Infrastructure.Persistence;
using Reminlo.Infrastructure.Repositories;
using Reminlo.Infrastructure.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RepositoryKit.Core.Interfaces;
using Scrutor;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Reminlo.Infrastructure;

/// <summary>
/// Provides extension methods to register infrastructure-layer dependencies.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers infrastructure services, including DbContext, Identity, caching, health checks, and custom services.
    /// </summary>
    /// <param name="services">The service collection to add dependencies to.</param>
    /// <param name="connectionString">The database connection string.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Register EnvService for environment variable management
        var envFilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
            ".env");
        services.AddSingleton<IEnvService>(provider =>
            new EnvService(envFilePath, provider.GetRequiredService<ILogger<EnvService>>()));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());

        services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpContextAccessor();
        services.AddMemoryCache();

        // Health Checks
        services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>("DbContext")
            .AddNpgSql(connectionString, name: "PostgreSQL");

        services.AddHealthChecksUI()
            .AddInMemoryStorage();

        services.Scan(action =>
        {
            action
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsMatchingInterface()
            .AsImplementedInterfaces()
            .WithScopedLifetime();
        });

        return services;
    }
}
