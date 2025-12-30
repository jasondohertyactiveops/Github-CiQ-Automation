using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AO.Automation.Helpers;

/// <summary>
/// Generates JWT tokens for test automation scenarios
/// Logic replicated from AO.WW.Core.Util.JwtIssuer to avoid cross-repo dependencies
/// </summary>
public class TokenHelper
{
    private readonly string _activationKey;
    private readonly string _resetPasswordKey;
    private const string Issuer = "workwareplus.com";
    private const string Audience = "workwareplus.com";
    
    public TokenHelper(string activationKey, string resetPasswordKey)
    {
        _activationKey = activationKey;
        _resetPasswordKey = resetPasswordKey;
    }
    
    /// <summary>
    /// Generate an account activation token
    /// Token format and signing matches AO.WW.Core.Util.JwtIssuer.ObtainActivationToken
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
    /// Token format and signing matches AO.WW.Core.Util.JwtIssuer.ObtainResetPasswordToken
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
    /// Create JWT token with specified claims and secret
    /// Replicates AO.WW.Core.Util.JwtIssuer.CreateToken logic
    /// </summary>
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
    
    /// <summary>
    /// Hash SecurityStamp using PBKDF2 (matches AO.WW.Core.Util.PasswordUtil.GenerateHashedPasswordString)
    /// Format: {salt}|{hash}
    /// </summary>
    private static string HashSecurityStamp(string securityStamp)
    {
        // Generate random 16-byte salt
        var saltBytes = new byte[16];
        RandomNumberGenerator.Fill(saltBytes);
        var salt = Convert.ToBase64String(saltBytes);
        
        // Hash using PBKDF2 with HMACSHA512 (10000 iterations, 256-bit output)
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
