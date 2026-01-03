# TC24154: Users Without Email Address (IsEmailRequired = false)

**Azure Test Case ID:** 24154  
**Suite:** Login-25146  
**Category:** ⚠️ Todo-NeedsReview  
**Reason:** ComplexWorkflow, SystemConfiguration, EnvironmentalComplexity

---

## Source Analysis

**Azure Test Case:**
- Step 1: Update system attribute in Support App and clear cache
- Step 2: Create users without email
- Step 3: Update passwords
- Step 4: Check French translation

**Cypress Implementation:**
- File: `cypress/e2e/UserJourney/UserWithEmailNotRequired.cy.js`
- Tests a complete user lifecycle WITHOUT email:
  1. Create user (via UI utility functions)
  2. Edit user  
  3. Password validation on change password screen
  4. Change password on first login
  5. Password reset by another user
  6. Delete user
- Uses specific client environment: `env: Cypress.env("reporting")`
- Assumes IsEmailRequired already set to false (no Support App config in test)

**Issues:**
- Azure says: Configure Support App + clear cache (not tested in Cypress)
- Azure says: Check French translation (not in Cypress)
- Cypress tests: Full user workflow assuming config already done
- **Gap:** Support App configuration not actually tested

**Playwright MCP:**
- Would need Support App access for system configuration
- Main app can create/edit users via Admin > Staff Members

---

## Decision

**Needs review before automation.**

**Questions:**
1. Should we test Support App configuration? (Not pure UI of main app)
2. Is this actually testing "users without email" or full user lifecycle?
3. French translation - is that UI validation or localization testing?
4. Should this be split into multiple tests?

**Options:**
- **A)** Test user CRUD in main app only (ignore Support App config, assume IsEmailRequired=false in test env)
- **B)** Mark as Not Pure UI (too complex, involves system config)
- **C)** Split into separate tests (user creation, password change, etc.)

---

## Automation Decision

**Requires discussion before proceeding.**

**Current implementation:** Cypress tests user lifecycle assuming IsEmailRequired=false already configured.

**Recommendation:** Review whether this belongs in Pure UI suite or should be split/moved.

**Playwright Pure UI:** Pending decision.