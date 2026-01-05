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
}
