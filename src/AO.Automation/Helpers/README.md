# Token Helper

Standalone JWT token generator for test automation.

## Purpose

Generates activation and reset password tokens at test runtime to avoid token expiry issues.

**Logic replicated from:** `AO.WW.Core.Util.JwtIssuer` (WW7 API codebase)

## Usage in Tests

```csharp
public class MyTest : PlaywrightTest
{
    [Fact]
    public async Task TestAccountActivation()
    {
        var tokenHelper = new TokenHelper(Config.JwtActivationKey, Config.JwtResetPasswordKey);
        
        // Generate fresh activation token
        var token = tokenHelper.GenerateActivationToken(
            clientIdentifier: "ww7client",
            staffMemberId: 9003,
            email: "tc24166.activation@activeops.com",
            securityStamp: "3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D"
        );
        
        // Use token immediately
        await Page.GotoAsync($"/activateaccount/{token}");
        
        // Token is fresh (just generated), won't expire during test
    }
}
```

## Configuration

JWT signing keys in `appsettings.json`:

```json
{
  "Jwt": {
    "ActivationKey": "s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3C",
    "ResetPasswordKey": "s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3D"
  }
}
```

**IMPORTANT:** These keys MUST match the WW7 API configuration:
- Local: `ww7-api/AO.WW/AO.WW.Web.Api.Client/appsettings.Containers.json`
- Keys: `JwtActivationKey` and `JwtResetPasswordKey`

## Token Types

### Activation Token
- **Used for:** Account activation flow
- **Expiry:** 24 hours (default)
- **Claims:** Email, ClientIdentifier, StaffMemberId, hashed SecurityStamp
- **URL format:** `/activateaccount/{token}`

### Reset Password Token
- **Used for:** Password reset flow
- **Expiry:** 24 hours (default)
- **Claims:** Username, ClientIdentifier, StaffMemberId, hashed SecurityStamp
- **URL format:** `/resetpassword/{token}`

## SecurityStamp Hashing

The SecurityStamp is hashed using SHA256 before being included in the token claims.
This matches the behavior in `AO.WW.Core.Util.PasswordUtil`.

## Notes

- Tokens are generated fresh every test run (no expiry issues)
- No dependency on WW7 API assemblies (standalone implementation)
- Uses standard Microsoft JWT libraries
- SecurityStamp must match the GUID in the User table for the test user
