using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace Reminlo.Api.Configurations;

public static class HealthCheckConfiguration
{
    public static IApplicationBuilder UseHealthCheckConfiguration(this IApplicationBuilder app)
    {
        // Configure health checks and UI endpoints
        app.UseHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.UseHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";
        });

        return app;
    }
}