# TC25058: Invalid Credentials Login API

**Azure Test Case:** 25058  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented (critical security gap)  
**Test Users:** 9200-9299 range (invalid password, inactive user, user without roles)

---

## What This Tests

API correctly rejects invalid login attempts (wrong password, inactive users, users without roles) and does not create session data.

---

## Test Steps

### Setup (Test Body - per scenario)
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, password }`

### Scenarios

Each scenario runs as a separate Theory case. Steps 1-3 are setup (arrange/act), Step 4 is the verification.

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | Prepare credentials for scenario | Setup |
| 2 | POST to /api/user/login | Setup |
| 3 | Count login records before attempt | Setup |
| 4 | Response returns 401, error present, no DB record created | Verify |

### Scenario Data

| Scenario | Username | Password | User ID | Condition |
|----------|----------|----------|---------|-----------|
| Invalid Password | api.tc25058.invalidpw@activeops.com | WrongPassword@1 | 9201 | Wrong password |
| Inactive User | api.tc25058.inactive@activeops.com | Workware@1 | 9202 | User is inactive |
| No Roles | api.tc25058.noroles@activeops.com | Workware@1 | 9203 | User has no roles |

---

## Gap Analysis

**Thunderclient Status:** ❌ **Completely missing - Critical security gap**

**What's Missing:**
- No invalid password tests
- No inactive user tests
- No user without roles tests
- No error response validation
- Zero negative testing for authentication

**Note:** The `01-login` folder contains `invalid-reset-password-user-doesnt-exist.json`, but this tests the `/Accounts/forgotpassword` endpoint (password reset), NOT the `/user/login` endpoint. Different functionality entirely.

**Why Critical:**
- Authentication security is foundational
- Invalid credential handling is common attack vector
- Error message handling can leak sensitive information
- Should be tested BEFORE valid login, not after

**Priority:** 🔴 **Implement immediately** (Priority 1)

---

## Notes

- Critical security test - missing from Thunderclient
- Should be Priority 1 for implementation
- Error messages should be generic to avoid information leakage
- Failed attempts may be logged in separate audit table
