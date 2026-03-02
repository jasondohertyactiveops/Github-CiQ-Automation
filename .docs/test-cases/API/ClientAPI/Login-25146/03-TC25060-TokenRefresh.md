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

## Test Steps

### Setup (Fixture)

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | POST /api/user/login with valid credentials | Setup |
| 2 | Capture access token and refresh token | Setup |
| 3 | Extract SessionValidationToken from access token | Setup |
| 4 | Generate expired access token using TokenHelper (-5 min) | Setup |
| 5 | PUT /api/user/login with expired token + valid refresh token | Setup |

### Response Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 6 | Status code is 200 | Verify |
| 7 | Response contains new token | Verify |
| 8 | Response contains new refreshToken | Verify |
| 9 | New token is different from expired token | Verify |
| 10 | New refresh token is different from old refresh token | Verify |
| 11 | New token is valid JWT format | Verify |
| 12 | New token expiry is ~30 min from refresh time | Verify |

### Database Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 13 | Login record created/updated in [UserLoginDetail] | Verify |
| 14 | New RefreshToken in DB matches response | Verify |
| 15 | New RefreshToken is different from old one in DB | Verify |
| 16 | New RefreshTokenExpiry is ~90 min from refresh time | Verify |

### End-to-End Validation

| Step | Description | Coverage |
|------|-------------|----------|
| 17 | New token can authenticate (POST /api/user/logout returns 204) | Verify |

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
