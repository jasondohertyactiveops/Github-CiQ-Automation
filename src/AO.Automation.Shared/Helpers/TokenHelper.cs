using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AO.Automation.Shared.Helpers;

/// <summary>
/// Generates JWT tokens and refresh tokens for test automation scenarios
/// Logic replicated from AO.WW.Core.Util.JwtIssuer to avoid cross-repo dependencies
/// </summary>
public class TokenHelper
{
    private readonly string _activationKey;
    private readonly string _resetPasswordKey;
    private readonly string _accessTokenKey;
    private const string Issuer = "workwareplus.com";
    private const string Audience = "workwareplus.com";
    
    /// <summary>
    /// Initialize TokenHelper with JWT signing keys
    /// </summary>
    /// <param name="activationKey">Key for activation tokens (JwtActivationKey)</param>
    /// <param name="resetPasswordKey">Key for reset password tokens (JwtResetPasswordKey)</param>
    /// <param name="accessTokenKey">Key for access tokens (JwtSecurityKey)</param>
    public TokenHelper(string activationKey, string resetPasswordKey, string accessTokenKey)
    {
        _activationKey = activationKey;
        _resetPasswordKey = resetPasswordKey;
        _accessTokenKey = accessTokenKey;
    }
    
    /// <summary>
    /// Generate an account activation token
    /// </summary>
    /// <param name="clientIdentifier">Client identifier (e.g., ww7client)</param>
    /// <param name="staffMemberId">Staff member ID</param>
    /// <param name="email">User's email address</param>
    /// <param name="securityStamp">User's security stamp from database</param>
    /// <param name="expiryMinutes">Token expiry in minutes from now (default: 1440 = 24 hours)</param>
    public string GenerateActivationToken(
        string clientIdentifier,
        int staffMemberId,
        string email,
        string securityStamp,
        int expiryMinutes = 1440)
    {
        var hashedSecurityStamp = HashSecurityStamp(securityStamp);
        
        var claims = new Dictionary<string, object>
        {
            { ClaimTypes.Name, email },
            { "ClientIdentifier", clientIdentifier },
            { "StaffMemberId", staffMemberId.ToString() },
            { "SecurityStamp", hashedSecurityStamp }
        };
        
        return CreateToken(claims, _activationKey, expiryMinutes);
    }
    
    /// <summary>
    /// Generate a password reset token
    /// </summary>
    /// <param name="clientIdentifier">Client identifier (e.g., ww7client)</param>
    /// <param name="staffMemberId">Staff member ID</param>
    /// <param name="username">Username</param>
    /// <param name="securityStamp">User's security stamp from database</param>
    /// <param name="expiryMinutes">Token expiry in minutes from now (default: 1440 = 24 hours)</param>
    public string GenerateResetPasswordToken(
        string clientIdentifier,
        int staffMemberId,
        string username,
        string securityStamp,
        int expiryMinutes = 1440)
    {
        var hashedSecurityStamp = HashSecurityStamp(securityStamp);
        
        var claims = new Dictionary<string, object>
        {
            { ClaimTypes.Name, username },
            { "ClientIdentifier", clientIdentifier },
            { "StaffMemberId", staffMemberId.ToString() },
            { "SecurityStamp", hashedSecurityStamp }
        };
        
        return CreateToken(claims, _resetPasswordKey, expiryMinutes);
    }
    
    /// <summary>
    /// Generate an access token for API authentication
    /// </summary>
    /// <param name="username">Username to encode in token</param>
    /// <param name="staffMemberId">Staff member ID</param>
    /// <param name="clientIdentifier">Client identifier (e.g., ww7client)</param>
    /// <param name="location">Timezone location (default: Europe/London)</param>
    /// <param name="sessionValidationToken">Session validation token (defaults to random GUID if not provided)</param>
    /// <param name="expiryMinutes">Token expiry in minutes from now (default: 30). Use negative values for expired tokens (e.g., -5 = expired 5 minutes ago)</param>
    public string GenerateAccessToken(
        string username,
        int staffMemberId,
        string clientIdentifier,
        string location = "Europe/London",
        string? sessionValidationToken = null,
        int expiryMinutes = 30)
    {
        var claims = new Dictionary<string, object>
        {
            { ClaimTypes.Name, username },
            { "ClientIdentifier", clientIdentifier },
            { "StaffMemberId", staffMemberId.ToString() },
            { "StaffMemberLocation", location },  // Note: Claim is StaffMemberLocation, not Location
            { "SessionValidationToken", sessionValidationToken ?? Guid.NewGuid().ToString() }
        };
        
        return CreateToken(claims, _accessTokenKey, expiryMinutes);
    }
    
    /// <summary>
    /// Generate a refresh token (random base64 string)
    /// </summary>
    /// <returns>64-character base64 refresh token matching database NVARCHAR(64) constraint</returns>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
    
    private string CreateToken(IDictionary<string, object> claims, string secret, int expirationMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryTime = DateTime.UtcNow.AddMinutes(expirationMinutes);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = Issuer,
            Audience = Audience,
            Claims = claims,
            Expires = expiryTime,
            SigningCredentials = creds
        };
        
        return new JsonWebTokenHandler().CreateToken(tokenDescriptor);
    }
    
    private static string HashSecurityStamp(string securityStamp)
    {
        var saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        
        var hash = Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivation.Pbkdf2(
            password: securityStamp,
            salt: saltBytes,
            prf: Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8);
        
        var hashString = Convert.ToBase64String(hash);
        
        return $"{salt}|{hashString}";
    }
}
