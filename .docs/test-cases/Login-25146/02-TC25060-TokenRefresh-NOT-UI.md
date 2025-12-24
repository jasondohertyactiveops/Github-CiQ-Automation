# TC25060: Login Token Refresh on Expiry

**Azure Test Case ID:** 25060  
**Suite:** Login-25146  
**Category:** ❌ Not Pure UI Appropriate  
**Reason:** BackendAuth, NFR

---

## Source Analysis

**Azure Test Case:**
- Single step: "User's refresh token should expire 60 min after token expiration time"
- No detailed steps provided
- Non-functional requirement (timing/expiration)

**Cypress Implementation:**
- File: `cypress/e2e/Global/LoginAndRefreshTokens.cy.js`
- Test: "should be able to refresh the token"
- What it does:
  - Logs in via API (`cy.loginByAPI()`) - receives token + refreshToken
  - Uses refreshToken as lookup ID
  - Queries database: `select RefreshTokenExpiry from UserLoginDetail where RefreshToken = '{refreshToken}'`
  - Compares database expiry timestamp vs token expiry
  - Verifies refresh token expires 90+ minutes after regular token expiry
- **This is backend database verification, not UI behavior**
- Refresh token is just an ID for database lookup, not parsed

**UI Feasibility:**
- Tests backend token refresh mechanism
- No UI component to verify
- Timing/expiration logic is backend concern
- User never sees token refresh happening

---

## Decision

**Not suitable for Pure UI automation.**

This tests backend authentication infrastructure and NFR (timing requirements).

---

## Automation Decision

**Not suitable for Pure UI automation.**

**Current implementation:** Cypress (`LoginAndRefreshTokens.cy.js`) tests token expiration timing via database queries and JWT parsing.

**Recommendation:** Leave in Cypress for now. Migrate to API test suite in Phase 2.

**Playwright Pure UI:** No test needed.
