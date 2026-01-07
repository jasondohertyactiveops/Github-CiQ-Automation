# TC25060: Token Refresh API

**Azure Test Case:** 25060  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented  
**Test Users:** 9204 (api.tc25060.tokenrefresh@activeops.com)  
**Status:** ✅ **Implemented**

---

## What This Tests

Token refresh endpoint allows obtaining new access token using expired token + valid refresh token, enabling sessions to continue without re-login.

---

## Test Checklist

### Initial Login
- [x] POST /api/user/login with valid credentials
- [x] Capture access token (30 min expiry)
- [x] Capture refresh token (90 min expiry)
- [x] Extract SessionValidationToken from access token for reuse

### Generate Expired Token
- [x] Use TokenHelper to create expired access token (expired 5 mins ago)
- [x] Reuse SessionValidationToken from real login (required for validation)
- [x] Match all claims from real token (username, staffMemberId, clientIdentifier, location)

### Token Refresh Request
- Method: PUT
- Endpoint: `/api/user/login`
- Body: `{ token: expired_token, refreshToken: valid_refresh_token }`

### Response Checks
- [x] Status code is 200
- [x] Response contains: token, refreshToken
- [x] New token is valid JWT (3-part structure)
- [x] New token is different from expired token
- [x] New refresh token is different from old refresh token
- [x] New token expiry is ~30 min from refresh time

### Database Checks
- [x] **UserLoginDetail:**
  - Record updated with new refresh token
  - New RefreshToken matches response
  - New RefreshToken is different from old one
  - RefreshTokenExpiry is ~90 min from refresh time

### End-to-End Validation
- [x] New token can be used for authenticated API requests
- [x] Tested by calling POST /api/user/logout with new token
- [x] Returns 204 No Content (token accepted, logout succeeded)

---

## Implementation Notes

**Token Generation:**
- Uses `TokenHelper.GenerateAccessToken()` with negative expiry
- Must reuse `SessionValidationToken` from real login (not random GUID)
- Claim must be `StaffMemberLocation` (not `Location`)

**Configuration:**
- Access token expiry: 30 minutes (`access-token-expiry`)
- Refresh token expiry: 90 minutes (`ww7client-timeout-general`)
- 60-minute window for token refresh after access token expires

**Swagger Discrepancy:**
- Logout endpoint: Swagger documents 202, API returns 204
- Test expects actual behavior (204)

---

## Gap Analysis

**Thunderclient Status:** ❌ **Completely missing**

**What Was Missing:**
- Token refresh endpoint not tested at all
- Only workaround was re-login (defeats purpose of refresh tokens)
- No validation of token timing
- No database verification
- Security-critical functionality untested

**Now Implemented:**
- ✅ Full token refresh flow
- ✅ Expired token generation for testing
- ✅ Database verification of refresh token updates
- ✅ End-to-end validation (new token actually works)
- ✅ Timing validation (30 min access, 90 min refresh)
