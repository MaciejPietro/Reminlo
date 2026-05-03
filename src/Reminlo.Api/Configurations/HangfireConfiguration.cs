using Hangfire;
using Reminlo.Infrastructure.Extensions;
using Reminlo.Infrastructure.Filters;

namespace Reminlo.Api.Configurations;

public static class HangfireConfiguration
{
    public static IApplicationBuilder UseHangfireConfiguration(this IApplicationBuilder app)
    {
        // Configure Hangfire dashboard with Basic Authentication filter
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = new[] { new BasicAuthAuthorizationFilter() }
        });

        // Use registered Hangfire recurring jobs
        app.UseCustomHangfireJobs();

        return app;
    }
}