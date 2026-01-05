# TC25060: Token Refresh API

**Azure Test Case:** 25060  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented (only has re-login workaround)  
**Test Users:** 9200-9299 range (active user with roles)

---

## What This Tests

Login creates refresh token with correct expiry timing (90 min from login, 60 min after main token expires).

---

## Test Checklist

### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, password }`

### Response Checks
- [ ] Status code is 200
- [ ] Response contains: token, refreshToken
- [ ] Token is valid JWT
- [ ] Token expiry is reasonable (around 30 min)
- [ ] RefreshToken is valid GUID

### Database Checks
- [ ] **UserLoginDetail:**
  - Login record created
  - RefreshToken matches response
  - RefreshTokenExpiry is reasonable (around 90 min from login)
  - RefreshTokenExpiry is approximately 60 min AFTER token expiry

---

## Future: Token Refresh Endpoint (Bonus)

Once refresh token endpoint is identified:

### Request
- Method: POST (TBD)
- Endpoint: `/api/user/refresh-token` (TBD)
- Body: `{ refreshToken }`

### Response Checks
- [ ] Status code is 200
- [ ] New token returned
- [ ] New token has fresh expiry

### Database Checks
- [ ] **UserLoginDetail:**
  - New/updated record with fresh RefreshTokenExpiry

---

## Gap Analysis

**Thunderclient Status:** ❌ **Missing - Only has re-login workaround**

**What Exists:**
- `07-planning/relogin-for-token-refresh/login.json` - just calls login again
- This is a workaround, not actual token refresh testing

**What's Missing:**
- No actual refresh token endpoint identified or tested
- No token expiry validation
- No refresh token expiry timing validation
- No database verification of token timing
- Refresh mechanism completely untested

**Why Critical:**
- Core session management functionality
- Token timing directly impacts security
- Currently only tested in Cypress (wrong layer)

**Priority:** 🔴 **Critical** (Priority 2 - after invalid login)

**Action Items:**
- Identify actual refresh token endpoint
- Migrate Cypress timing tests to proper API layer

---

## Notes

- Was marked "NOT-UI" - perfect for API testing
- Currently only tested in Cypress via DB queries (wrong layer)
- Refresh token endpoint needs to be identified
- Timing validation is core security requirement
