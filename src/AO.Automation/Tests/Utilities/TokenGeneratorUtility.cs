using AO.Automation.BaseClasses;
using AO.Automation.Helpers;

namespace AO.Automation.Tests.Utilities;

/// <summary>
/// Manual utility to generate tokens for debugging
/// Run with: dotnet test --filter "FullyQualifiedName~GenerateActivationToken"
/// </summary>
public class TokenGeneratorUtility : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public TokenGeneratorUtility(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public void GenerateActivationToken()
    {
        var tokenHelper = new TokenHelper(Config.JwtActivationKey, Config.JwtResetPasswordKey);
        
        var token = tokenHelper.GenerateActivationToken(
            clientIdentifier: "ww7client",
            staffMemberId: 9003,
            email: "tc24166.activation@activeops.com",
            securityStamp: "3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D"
        );
        
        var url = $"http://ww7client.localhost/activateaccount/{token}";
        
        Console.WriteLine("=====================================");
        Console.WriteLine("ACTIVATION TOKEN GENERATED");
        Console.WriteLine("=====================================");
        Console.WriteLine();
        Console.WriteLine("Token:");
        Console.WriteLine(token);
        Console.WriteLine();
        Console.WriteLine("URL:");
        Console.WriteLine(url);
        Console.WriteLine();
        Console.WriteLine("Valid for: 24 hours");
        Console.WriteLine("=====================================");
        
        // Force test to "pass" so we can see the output
        Assert.True(true);
    }
}
