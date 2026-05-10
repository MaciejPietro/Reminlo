using Reminlo.Application;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Infrastructure;
using Reminlo.Infrastructure.Extensions;
using Reminlo.Infrastructure.Middlewares;
using Reminlo.Infrastructure.Persistence;
using Reminlo.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;
using Reminlo.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Configure logging with Serilog
builder.Logging.AddConsole();
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
builder.Host.UseSerilog(logger);

// Get connection string from .env file for security
var envFilePath = Path.Combine(
    Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "",
    "..", "..", "..", "..", "Reminlo.Infrastructure", ".env");
var envService = new EnvService(envFilePath, new LoggerFactory().CreateLogger<EnvService>());
var connectionString = envService.GetValue("DB_CONNECTION_STRING")
    ?? throw new InvalidOperationException("DB_CONNECTION_STRING not found in .env file");

// Configure CORS to allow all origins, headers, and methods
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Only vite app for now
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register application and infrastructure services, including DbContext and DI
builder.Services.AddApplication(); // Registers MediatR and application services
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddAuthConfiguration(builder.Configuration);
builder.Services.AddExceptionsHandler();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<Reminlo.Api.Filters.ErrorOrResultFilter>();
    options.Filters.Add<Reminlo.Api.Filters.ApiResponseWrapperFilter>();
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new Reminlo.Api.Converters.StronglyTypedIdJsonConverterFactory());
});

builder.Services.AddSwaggerConfiguration();

// Add and configure Hangfire background job services
builder.Services.AddCustomHangfireJobs(connectionString);

builder.Services.AddRateLimiterConfiguration();

var app = builder.Build();

// Apply pending EF Core migrations and seed data on startup
await app.ApplyMigrationsAndSeedAsync();

// Development-only middleware for Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register global exception handling middleware
app.UseExceptionHandler();

app.UseHttpsRedirection();

// Enable CORS policy before authentication and authorization middleware
app.UseCors("AllowAll");

app.UseAuthConfiguration();


// Register custom audit logging middleware
app.UseAuditLogging();

app.UseHealthCheckConfiguration();

app.UseHangfireConfiguration();

// Enable rate limiting middleware
app.UseRateLimiter();

// Map controllers and apply rate limiting policy globally
app.MapControllers()
   .RequireRateLimiting("fixed");

app.Run();
