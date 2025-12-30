using Microsoft.Extensions.Configuration;
using System;

namespace AO.Automation.Config;

/// <summary>
/// Reads and provides access to test configuration settings from appsettings.json
/// </summary>
public class TestConfig
{
    private static TestConfig? _instance;
    private readonly IConfiguration _configuration;

    private TestConfig()
    {
        var environment = System.Environment.GetEnvironmentVariable("TEST_ENVIRONMENT") ?? "Local";
        
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    public static TestConfig Instance => _instance ??= new TestConfig();

    public string BaseUrl => _configuration["BaseUrl"] ?? throw new InvalidOperationException("BaseUrl not configured");
    public string Environment => _configuration["Environment"] ?? "Unknown";
    public string Browser => _configuration["Browser"] ?? "chromium";
    public bool Headless => bool.Parse(_configuration["Headless"] ?? "true");
    
    public int DefaultTimeout => int.Parse(_configuration["Timeout:Default"] ?? "30000");
    public int NavigationTimeout => int.Parse(_configuration["Timeout:Navigation"] ?? "60000");
    
    public bool ScreenshotsOnFailure => bool.Parse(_configuration["Screenshots:OnFailure"] ?? "true");
    public string ScreenshotsPath => _configuration["Screenshots:Path"] ?? "screenshots";
    
    public bool TracesOnFailure => bool.Parse(_configuration["Traces:OnFailure"] ?? "true");
    public string TracesPath => _configuration["Traces:Path"] ?? "traces";
    
    public string JwtActivationKey => _configuration["Jwt:ActivationKey"] ?? throw new InvalidOperationException("Jwt:ActivationKey not configured");
    public string JwtResetPasswordKey => _configuration["Jwt:ResetPasswordKey"] ?? throw new InvalidOperationException("Jwt:ResetPasswordKey not configured");
    
    public string GetTestUserUsername(string userType) => 
        _configuration[$"TestUsers:{userType}:Username"] ?? throw new InvalidOperationException($"TestUsers:{userType}:Username not configured");
    
    public string GetTestUserPassword(string userType) => 
        _configuration[$"TestUsers:{userType}:Password"] ?? throw new InvalidOperationException($"TestUsers:{userType}:Password not configured");
    
    public string GetTestUserAuthStatePath(string userType) => 
        _configuration[$"TestUsers:{userType}:AuthStatePath"] ?? $"Fixtures/auth-{userType.ToLower()}.json";
}
