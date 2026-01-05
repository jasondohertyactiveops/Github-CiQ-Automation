# TC25058: Invalid Credentials Login API

**Azure Test Case:** 25058  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented (critical security gap)  
**Test Users:** 9200-9299 range (invalid password, inactive user, user without roles)

---

## What This Tests

API correctly rejects invalid login attempts (wrong password, inactive users, users without roles) and does not create session data.

---

## Test Checklist

### Scenario 1: Invalid Password

#### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, wrong_password }`

#### Response Checks
- [ ] Status code is 401
- [ ] Error message exists
- [ ] No token returned
- [ ] Error doesn't reveal if username exists

#### Database Checks
- [ ] **UserLoginDetail:**
  - No login record created

---

### Scenario 2: Inactive User

#### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, correct_password }`
- User state: Inactive (Active = 0)

#### Response Checks
- [ ] Status code is 401
- [ ] Error message indicates inactive account
- [ ] No token returned

#### Database Checks
- [ ] **UserLoginDetail:**
  - No login record created

---

### Scenario 3: User Without Roles

#### Request
- Method: POST
- Endpoint: `/api/user/login`
- Body: `{ clientIdentifier, username, correct_password }`
- User state: No role assignments

#### Response Checks
- [ ] Status code is 401 or 403
- [ ] Error message indicates missing roles
- [ ] No token returned

#### Database Checks
- [ ] **UserLoginDetail:**
  - No login record created

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
