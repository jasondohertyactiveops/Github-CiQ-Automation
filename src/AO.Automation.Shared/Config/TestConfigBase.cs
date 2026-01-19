using System;
using Microsoft.Extensions.Configuration;

namespace AO.Automation.Shared.Config;

/// <summary>
/// Base test configuration for both API and UI test projects
/// Reads shared settings from appsettings.json and environment variables
/// Extend this class in specific test projects to add project-specific config
/// </summary>
public class TestConfigBase
{
    protected readonly IConfiguration _configuration;

    protected TestConfigBase()
    {
        var environment = System.Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") 
            ?? System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") 
            ?? "Local";
        
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    // Common settings
    public string BaseUrl => _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not configured");
    public string Environment => _configuration["Environment"] ?? "Unknown";
    
    // JWT Keys for token generation
    public string JwtSecurityKey => _configuration["Jwt:SecurityKey"] ?? throw new InvalidOperationException("Jwt:SecurityKey not configured");
    public string JwtActivationKey => _configuration["Jwt:ActivationKey"] ?? throw new InvalidOperationException("Jwt:ActivationKey not configured");
    public string JwtResetPasswordKey => _configuration["Jwt:ResetPasswordKey"] ?? throw new InvalidOperationException("Jwt:ResetPasswordKey not configured");
    
    // Timeout settings
    public int DefaultTimeout => int.Parse(_configuration["Timeout:Default"] ?? "30000");
    public int NavigationTimeout => int.Parse(_configuration["Timeout:Navigation"] ?? "60000");
    
    // Screenshot settings
    public bool ScreenshotsOnFailure => bool.Parse(_configuration["Screenshots:OnFailure"] ?? "true");
    public string ScreenshotsPath => _configuration["Screenshots:Path"] ?? "screenshots";
    
    // Trace settings
    public bool TracesOnFailure => bool.Parse(_configuration["Traces:OnFailure"] ?? "true");
    public string TracesPath => _configuration["Traces:Path"] ?? "traces";
    
    // Browser settings (primarily for UI tests, but harmless for API)
    public string Browser => _configuration["Browser"] ?? "chromium";
    public bool Headless => bool.Parse(_configuration["Headless"] ?? "true");
    
    // Test user helpers
    public string GetTestUserUsername(string userType) => 
        _configuration[$"TestUsers:{userType}:Username"] ?? throw new InvalidOperationException($"TestUsers:{userType}:Username not configured");
    
    public string GetTestUserPassword(string userType) => 
        _configuration[$"TestUsers:{userType}:Password"] ?? throw new InvalidOperationException($"TestUsers:{userType}:Password not configured");
    
    public string GetTestUserAuthStatePath(string userType) => 
        _configuration[$"TestUsers:{userType}:AuthStatePath"] ?? $"Fixtures/auth-{userType.ToLower()}.json";
}
