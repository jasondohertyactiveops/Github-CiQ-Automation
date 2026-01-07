using Microsoft.Extensions.Configuration;

namespace AO.Automation.API.Client.Config;

/// <summary>
/// Configuration settings for API tests
/// </summary>
public class ApiTestConfig
{
    private static ApiTestConfig? _instance;
    private readonly IConfiguration _configuration;
    
    public static ApiTestConfig Instance => _instance ??= new ApiTestConfig();
    
    private ApiTestConfig()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Local";
        
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
    }
    
    public string ApiBaseUrl => _configuration["Api:BaseUrl"] 
        ?? throw new InvalidOperationException("Api:BaseUrl not configured");
    
    public string DatabaseConnectionString => _configuration["Database:ConnectionString"]
        ?? throw new InvalidOperationException("Database:ConnectionString not configured");
    
    // JWT keys from container config (match appsettings.Containers.json)
    public string JwtSecurityKey => "s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3B";
    public string JwtActivationKey => "s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3C";
    public string JwtResetPasswordKey => "s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3D";
    
    /// <summary>
    /// Get TokenHelper configured with correct JWT keys for token generation
    /// </summary>
    public AO.Automation.Shared.Helpers.TokenHelper GetTokenHelper()
    {
        return new AO.Automation.Shared.Helpers.TokenHelper(
            JwtActivationKey, 
            JwtResetPasswordKey, 
            JwtSecurityKey);
    }
}
