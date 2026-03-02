# TC25057: Valid Login API

**Azure Test Case:** 25057  
**Suite:** Login-25146  
**Thunderclient:** `client-site-v2.1/01-login/login.json` ✅  
**Test Users:** 9200-9299 range (active user with roles)

---

## What This Tests

User with valid credentials can successfully login via API and receive a valid JWT token with correct database state.

---

## Test Steps

### Setup (Fixture)
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, password }`

### Response Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | Status code is 200 | Verify |
| 2 | Response contains token | Verify |
| 3 | Response contains refreshToken | Verify |
| 4 | Token is valid JWT format | Verify |
| 5 | Token contains expected claims (name, ClientIdentifier) | Verify |
| 6 | Token expiry is ~30 minutes | Verify |
| 7 | Response contains user details (username, firstName, lastName, location) | Verify |

### Database Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 8 | User record exists in [User] table | Verify |
| 9 | Login record created in [UserLoginDetail] | Verify |
| 10 | Login timestamp is recent (within 30s) | Verify |
| 11 | RefreshToken in DB matches response | Verify |
| 12 | RefreshTokenExpiry is ~90 minutes | Verify |

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
