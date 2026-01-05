using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AO.Automation.Shared.Helpers;

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
