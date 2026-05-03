using Microsoft.Extensions.Logging;

namespace Reminlo.Infrastructure.Services;

/// <summary>
/// Service for managing environment variables loaded from a .env file.
/// </summary>
public interface IEnvService
{
    /// <summary>
    /// Gets an environment variable value by key.
    /// </summary>
    /// <param name="key">The environment variable key.</param>
    /// <param name="defaultValue">Optional default value if key is not found.</param>
    /// <returns>The environment variable value or default value if not found.</returns>
    string? GetValue(string key, string? defaultValue = null);
}

/// <summary>
/// Implementation of environment variable service that loads from a .env file.
/// </summary>
public class EnvService : IEnvService
{
    private readonly Dictionary<string, string> _variables;
    private readonly ILogger<EnvService> _logger;

    /// <summary>
    /// Initializes a new instance of the EnvService class.
    /// </summary>
    /// <param name="envFilePath">Path to the .env file.</param>
    /// <param name="logger">Logger instance.</param>
    public EnvService(string envFilePath, ILogger<EnvService> logger)
    {
        _logger = logger;
        _variables = new Dictionary<string, string>();

        if (File.Exists(envFilePath))
        {
            LoadEnvFile(envFilePath);
            _logger.LogInformation("Environment variables loaded from {FilePath}", envFilePath);
        }
        else
        {
            _logger.LogWarning("Environment file not found at {FilePath}", envFilePath);
        }
    }

    /// <summary>
    /// Gets an environment variable value by key.
    /// </summary>
    public string? GetValue(string key, string? defaultValue = null)
    {
        if (_variables.TryGetValue(key, out var value))
        {
            return value;
        }

        // Fall back to system environment variables
        var envValue = Environment.GetEnvironmentVariable(key);
        if (envValue != null)
        {
            return envValue;
        }

        return defaultValue;
    }

    /// <summary>
    /// Loads and parses the .env file.
    /// </summary>
    private void LoadEnvFile(string filePath)
    {
        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Skip empty lines and comments
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                {
                    continue;
                }

                // Parse key=value format
                var parts = trimmedLine.Split('=', 2);
                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Remove quotes if present
                    if ((value.StartsWith('"') && value.EndsWith('"')) ||
                        (value.StartsWith('\'') && value.EndsWith('\'')))
                    {
                        value = value[1..^1];
                    }

                    _variables[key] = value;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading environment file from {FilePath}", filePath);
            throw;
        }
    }
}
