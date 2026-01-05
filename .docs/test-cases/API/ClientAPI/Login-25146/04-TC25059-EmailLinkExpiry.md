# TC25059: Email Activation Link 24hr Validity API

**Azure Test Case:** 25059  
**Suite:** Login-25146  
**Thunderclient:** ❌ Not implemented  
**Test Users:** 9200-9299 range (invited status, no password set)  
**Source:** Azure DevOps test suite (marked as "NOT-UI" during UI analysis)

---

## What This Tests

Activation tokens expire after 24 hours and new tokens invalidate old ones.

---

## Test Checklist

### Scenario 1: Valid Token (Within 24 Hours)

#### Request
- Method: POST (TBD - endpoint needs identification)
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

### Scenario 3: Old Token Invalidated by New Token

#### Request
- Generate first token
- Generate second token  
- Attempt activation with first token

#### Response Checks
- [ ] First token rejected (400/401)
- [ ] Second token works successfully

---

## Gap Analysis

**Thunderclient Status:** ❌ **Not from Thunderclient - Azure DevOps test case**

**Context:**
- This test case is from the Azure DevOps Login-25146 suite
- Was marked "NOT-UI" during UI test case analysis
- Recommended for API test suite implementation
- Does NOT exist in Thunderclient collection

**What Needs to Be Built:**
- Identify activation endpoint
- Implement activation token validation tests
- Implement expiry testing (requires time manipulation strategy)
- Implement token invalidation testing

**Why This is Different:**
- Not a Thunderclient migration - this is **net new** test creation
- Based on Azure requirements, not existing API tests
- Fills gap in authentication/onboarding flow coverage

**Priority:** 🟡 **Medium** (Priority 3)

**Action Items:**
- Identify activation endpoint in codebase
- Choose time manipulation approach (TokenHelper with past expiry recommended)
- Understand token storage mechanism (JWT vs database table)

---

## Notes

- User onboarding security test
- Time-based testing requires strategy decision  
- Currently untested in any automation (Thunderclient or Cypress)
- Lower priority than core login/auth flows
