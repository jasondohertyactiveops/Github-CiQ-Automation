namespace AO.Automation.Shared.Utilities;

/// <summary>
/// Utility for generating tokens for debugging purposes
/// Can be called from UI or API test projects
/// </summary>
public static class TokenGenerator
{
    /// <summary>
    /// Generate and print activation token for debugging
    /// </summary>
    public static void GenerateActivationToken(
        string activationKey,
        string clientIdentifier,
        int staffMemberId,
        string email,
        string securityStamp)
    {
        var tokenService = new Helpers.TokenService(activationKey, string.Empty);
        
        var token = tokenService.GenerateActivationToken(
            clientIdentifier,
            staffMemberId,
            email,
            securityStamp
        );
        
        var url = $"http://{clientIdentifier}.localhost/activateaccount/{token}";
        
        Console.WriteLine("=====================================");
        Console.WriteLine("ACTIVATION TOKEN GENERATED");
        Console.WriteLine("=====================================");
        Console.WriteLine();
        Console.WriteLine($"Staff Member ID: {staffMemberId}");
        Console.WriteLine($"Email: {email}");
        Console.WriteLine();
        Console.WriteLine("Token:");
        Console.WriteLine(token);
        Console.WriteLine();
        Console.WriteLine("URL:");
        Console.WriteLine(url);
        Console.WriteLine();
        Console.WriteLine("Valid for: 24 hours");
        Console.WriteLine("=====================================");
    }
    
    /// <summary>
    /// Generate and print reset password token for debugging
    /// </summary>
    public static void GenerateResetPasswordToken(
        string resetPasswordKey,
        string clientIdentifier,
        int staffMemberId,
        string username,
        string securityStamp)
    {
        var tokenService = new Helpers.TokenService(string.Empty, resetPasswordKey);
        
        var token = tokenService.GenerateResetPasswordToken(
            clientIdentifier,
            staffMemberId,
            username,
            securityStamp
        );
        
        var url = $"http://{clientIdentifier}.localhost/resetpassword/{token}";
        
        Console.WriteLine("=====================================");
        Console.WriteLine("RESET PASSWORD TOKEN GENERATED");
        Console.WriteLine("=====================================");
        Console.WriteLine();
        Console.WriteLine($"Staff Member ID: {staffMemberId}");
        Console.WriteLine($"Username: {username}");
        Console.WriteLine();
        Console.WriteLine("Token:");
        Console.WriteLine(token);
        Console.WriteLine();
        Console.WriteLine("URL:");
        Console.WriteLine(url);
        Console.WriteLine();
        Console.WriteLine("Valid for: 24 hours");
        Console.WriteLine("=====================================");
    }
}
