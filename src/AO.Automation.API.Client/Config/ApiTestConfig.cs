using AO.Automation.Shared.Config;
using Microsoft.Extensions.Configuration;

namespace AO.Automation.API.Client.Config;

/// <summary>
/// API-specific test configuration
/// Extends shared TestConfigBase with database connection string
/// </summary>
public class ApiTestConfig : TestConfigBase
{
    private static ApiTestConfig? _instance;
    
    public static ApiTestConfig Instance => _instance ??= new ApiTestConfig();
    
    private ApiTestConfig() : base()
    {
    }
    
    // API-ONLY: Direct database access (UI tests should NEVER have access to this)
    public string DatabaseConnectionString => _configuration["Database:ConnectionString"]
        ?? throw new InvalidOperationException("Database:ConnectionString not configured");
}
