using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Reminlo.Api.Configurations;

public static class RateLimiterConfiguration
{
    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("fixed", config =>
            {
                config.Window = TimeSpan.FromSeconds(10);    // 10-second window
                config.PermitLimit = 5;                       // Max 5 requests per window
                config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                config.QueueLimit = 2;                        // Queue up to 2 requests beyond limit
            });
        });

        return services;
    }
}