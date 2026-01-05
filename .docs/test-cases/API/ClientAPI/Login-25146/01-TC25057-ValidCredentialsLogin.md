# TC25057: Valid Login API

**Azure Test Case:** 25057  
**Suite:** Login-25146  
**Thunderclient:** `client-site-v2.1/01-login/login.json` ✅  
**Test Users:** 9200-9299 range (active user with roles)

---

## What This Tests

User with valid credentials can successfully login via API and receive a valid JWT token with correct database state.

---

## Test Checklist

### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, password }`

### Response Checks
- [ ] Status code is 200
- [ ] Response contains: token, refreshToken, user object
- [ ] Token is valid JWT format
- [ ] Token expiry is reasonable
- [ ] RefreshToken is valid GUID
- [ ] User ID matches expected user
- [ ] Username matches request

### Database Checks
- [ ] **UserLoginDetail:**
  - Login record created for test user
  - LoginDateTime is recent
  - RefreshToken matches response
  - RefreshTokenExpiry is reasonable
  - SessionValidationToken exists

---

## Gap Analysis

**Thunderclient Status:** ✅ Test exists but incomplete

**What's Missing:**
- No response schema validation (only checks status 200)
- No database verification (doesn't check UserLoginDetail record)
- Uses environment variables instead of dedicated test users

**Enhancement Opportunity:**
- Add comprehensive response validation
- Add database verification with proper SQL checks
- Use dedicated test users from 9200-9299 range

---

## Notes

- Token can be reused for authenticated API calls
- Complements UI login test (different layer)
- Thunderclient test exists but lacks DB verification
