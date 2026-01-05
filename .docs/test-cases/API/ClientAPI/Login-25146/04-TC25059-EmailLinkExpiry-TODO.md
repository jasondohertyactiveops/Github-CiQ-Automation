# TC25059: Email Activation Link 24hr Validity API

**Azure Test Case:** 25059  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented  
**Test Users:** 9200-9299 range (invited status, no password)  
**Status:** ⚠️ TODO - Requires time manipulation strategy and endpoint identification

---

## What This Tests

Activation tokens expire after 24 hours and new tokens invalidate old ones.

---

## Test Checklist

### Scenario 1: Valid Token (Within 24 Hours)

#### Request
- Method: POST (TBD)
- Endpoint: `/api/user/activate` (TBD)
- Body: `{ token, password }`

#### Response Checks
- [ ] Status code is 200
- [ ] Success message returned

#### Database Checks
- [ ] **User:**
  - ActivationStatusId changed to Active
  - PasswordHash is set

---

### Scenario 2: Expired Token (After 24 Hours)

#### Request
- Method: POST
- Endpoint: `/api/user/activate`
- Body: `{ expired_token, password }`

#### Response Checks
- [ ] Status code is 400 or 401
- [ ] Error indicates token expired

#### Database Checks
- [ ] **User:**
  - ActivationStatusId still Invited
  - PasswordHash still null

---

### Scenario 3: Old Token Invalidated

#### Request
- Generate first token
- Generate second token
- Attempt activation with first token

#### Response Checks
- [ ] First token rejected (400/401)
- [ ] Second token works successfully

---

---

## Gap Analysis

**Thunderclient Status:** ❌ **Completely missing**

**What's Missing:**
- No activation endpoint tests
- No activation token validation
- No token expiry testing
- No "new token invalidates old" testing
- Entire user activation flow untested

**Why Medium Priority:**
- Important for user onboarding security
- Time-based testing is complex (needs strategy)
- Not currently tested anywhere (not even Cypress)
- Lower priority than login/auth core flows

**Priority:** 🟡 **Medium** (Priority 3 - after core auth tests)

**Action Items:**
- Identify activation endpoint
- Decide on time manipulation approach for 24-hour expiry
- Understand token storage/validation mechanism

---

## Notes

- Time-based testing requires strategy decision
- Currently untested in any automation
- User onboarding security is important but not critical path
- Recommend Priority 3 (after invalid login and token refresh)
