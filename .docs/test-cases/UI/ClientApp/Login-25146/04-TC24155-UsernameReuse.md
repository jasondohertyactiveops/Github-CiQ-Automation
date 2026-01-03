# TC24155: Username Reuse After Deletion

**Azure Test Case ID:** 24155  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Step 1: "Username can be reused where original instance is deleted"
- Step 2: "Check the translated content in French"
- No detailed steps

**Cypress Implementation:**
- No dedicated test file found
- Likely tested as part of user CRUD workflow

**What This Should Test:**
- User can login with a username that was previously used by a deleted user
- System correctly identifies and authenticates the active user (not confused by deleted one)

---

## Final Test Specification

### What This Test Verifies
Login works correctly when username exists in both deleted and active states.

### Prerequisites
**Seeded database must contain:**
- User 1: Username "duplicate.test@activeops.com", Status: Deleted
- User 2: Username "duplicate.test@activeops.com", Status: Active, Password: [seeded password]

If seeding succeeds, database constraint allows duplicate usernames when one is deleted ✅

### Test Steps
1. Navigate to login page
2. Enter username: "duplicate.test@activeops.com"
3. Enter password: [seeded password]
4. Click Login
5. Verify: Redirects to dashboard (login successful)

### Expected Results
- Login succeeds
- System authenticates active user (not deleted one)
- Dashboard loads correctly

### Automation Approach
- **Pattern:** Pattern A (simple workflow)
- **Page Objects:** LoginPage, DashboardPage
- **Locators:**
  - Username: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })`
  - Password: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })`
  - Login button: `Page.GetByRole(AriaRole.Button, new() { Name = "Login" })`
- **Verification:** URL contains "/dashboard" or dashboard element visible

---

## Notes
- Seeding script validates database constraint (can insert duplicate usernames)
- UI test validates system handles authentication correctly with edge case data
- French translation check: Out of scope for Pure UI testing (localization testing)
