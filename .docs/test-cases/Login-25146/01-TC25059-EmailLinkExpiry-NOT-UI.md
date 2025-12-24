# TC25059: Email Activation Link 24hr Validity

**Azure Test Case ID:** 25059  
**Suite:** Login-25146  
**Category:** ❌ Not Pure UI Appropriate  
**Reason:** TimeBased, EmailSystem

---

## Source Analysis

**Azure Test Case:**
- Single step: "User's email activation link should be valid for 24 hours idle unless a new link is issued"
- No detailed steps provided

**Cypress Implementation:**
- File: `cypress/e2e/Global/LoginAndRefreshTokens.cy.js`
- Contains token refresh tests, NOT email link tests
- Tests found:
  - "should be able to parse the token" - Parses JWT, verifies 30min expiry
  - "should be able to refresh the token" - Queries database for refresh token timing
- Uses: `cy.loginByAPI()`, `cy.queryDatabase()`, JWT parsing
- These are backend API/auth tests, not UI tests
- No Cypress UI implementation found for TC25059

**UI Feasibility:**
- Requires 24-hour wait period
- Tests email delivery system
- Tests backend link generation/expiration logic
- Cannot be reliably verified via UI

---

## Decision

**Not suitable for UI automation.**

Should be implemented in API/Integration test suite where:
- Time can be mocked/accelerated
- Email system can be verified directly
- Link expiration logic can be tested without waiting

---

## Automation Decision

**Not suitable for Pure UI automation.**

**Current implementation:** Cypress (`LoginAndRefreshTokens.cy.js`) tests backend auth logic via JWT parsing and database queries.

**Recommendation:** Leave in Cypress for now. Migrate to API test suite in Phase 2.

**Playwright Pure UI:** No test needed.
