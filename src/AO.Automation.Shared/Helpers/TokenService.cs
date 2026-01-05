namespace AO.Automation.Shared.Helpers;

/// <summary>
/// Public facade for token generation
/// Abstracts TokenHelper implementation details from consuming projects
/// </summary>
public class TokenService
{
    private readonly TokenHelper _tokenHelper;
    
    public TokenService(string activationKey, string resetPasswordKey)
    {
        _tokenHelper = new TokenHelper(activationKey, resetPasswordKey);
    }
    
    /// <summary>
    /// Generate an account activation token
    /// </summary>
    /// <param name="clientIdentifier">Client identifier (e.g., "ww7client")</param>
    /// <param name="staffMemberId">StaffMember ID</param>
    /// <param name="email">User's email address</param>
    /// <param name="securityStamp">User's SecurityStamp GUID from database</param>
    /// <param name="expiryMinutes">Token expiry in minutes (default: 1440 = 24 hours)</param>
    /// <returns>JWT token string</returns>
    public string GenerateActivationToken(
        string clientIdentifier,
        int staffMemberId,
        string email,
        string securityStamp,
        int expiryMinutes = 1440)
    {
        return _tokenHelper.GenerateActivationToken(
            clientIdentifier, 
            staffMemberId, 
            email, 
            securityStamp, 
            expiryMinutes);
    }
    
    /// <summary>
    /// Generate a password reset token
    /// </summary>
    /// <param name="clientIdentifier">Client identifier (e.g., "ww7client")</param>
    /// <param name="staffMemberId">StaffMember ID</param>
    /// <param name="username">User's username</param>
    /// <param name="securityStamp">User's SecurityStamp GUID from database</param>
    /// <param name="expiryMinutes">Token expiry in minutes (default: 1440 = 24 hours)</param>
    /// <returns>JWT token string</returns>
    public string GenerateResetPasswordToken(
        string clientIdentifier,
        int staffMemberId,
        string username,
        string securityStamp,
        int expiryMinutes = 1440)
    {
        return _tokenHelper.GenerateResetPasswordToken(
            clientIdentifier, 
            staffMemberId, 
            username, 
            securityStamp, 
            expiryMinutes);
    }
}
