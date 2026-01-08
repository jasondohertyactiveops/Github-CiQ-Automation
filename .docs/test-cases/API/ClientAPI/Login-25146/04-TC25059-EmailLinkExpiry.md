# TC25059: Account Activation Token Validation

**Azure Test Case:** 25059  
**Suite:** Login-25146  
**Status:** ⏳ Ready to Implement  
**Type:** OneShot (user 9205 consumed)  
**User:** 9205 (api.tc25059.activation@activeops.com, SecurityStamp: 5A6B7C8D-9E0F-1A2B-3C4D-5E6F7A8B9C0D)

---

## What This Tests

Activation endpoint validates token expiry correctly:
- Expired tokens (>24 hours) are rejected
- Valid tokens (within 24 hours) activate users successfully

**Scope:** Testing `/api/Accounts/activate` endpoint (login flow). NOT testing admin resend or email sending.

---

## Scenarios

### 1. Expired Token (Run First)

**Token:** Generated with `expiryMinutes: -1500` (expired 25 hours ago)  
**Request:** POST /api/Accounts/activate with token in Authorization header  
**Body:** `{ username, password }`

**Expected:**
- Status: 401 Unauthorized
- User.ActivationStatusId: Still 2 (Invited)
- User.PasswordHash: Still NULL

### 2. Valid Token (Run Second)

**Token:** Generated with `expiryMinutes: 1440` (24 hours)  
**Request:** Same as above with valid token

**Expected:**
- Status: 200 OK
- User.ActivationStatusId: 2 → 3 (Active)
- User.PasswordHash: Set
- No UserLoginDetail record (activation ≠ login)

---

## Implementation

**Token Generation:**
```csharp
var tokenHelper = ApiTestConfig.Instance.GetTokenHelper();
var token = tokenHelper.GenerateActivationToken(
    clientIdentifier: "ww7client",
    staffMemberId: 9205,
    email: "api.tc25059.activation@activeops.com",
    securityStamp: "5A6B7C8D-9E0F-1A2B-3C4D-5E6F7A8B9C0D",
    expiryMinutes: 1440  // or -1500 for expired
);
```

**API Call:**
```csharp
await apiHelper.SetAuthTokenAsync(activationToken);
var response = await apiHelper.PostAsync<object>(
    "/api/Accounts/activate",
    new { username = "...", password = "..." });
```

**Pattern:** Use Pattern 1 (Fixture) - Both scenarios in SetupAsync, run expired first then valid.

---

## Database Queries

```sql
-- Check activation status and password
SELECT Id, ActivationStatusId, PasswordHash FROM [dbo].[User] WHERE Id = 9205

-- Verify no login session created
SELECT COUNT(*) FROM [dbo].[UserLoginDetail] WHERE UserId = 9205
```

---

## Notes

- Token uses JwtActivationKey (`s3cu1tyJwtT0k3nK3ys3cu1tyJwtT0k3C`)
- Default expiry: 24 hours (1440 minutes)
- Token in Authorization header, NOT in body
- SecurityStamp hashed by TokenHelper (PBKDF2 with salt)
- OneShot: Mark with `[Trait("Category", "OneShot")]`
