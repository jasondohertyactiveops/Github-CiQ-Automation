# TC25057: Valid Credentials Login

**Azure Test Case ID:** 25057  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Step 1: "User with valid credentials is able to login"
- Step 2: "Check the translated content in French"
- Vague, no details

**Cypress Implementation:**
- File: `cypress/e2e/Global/LoginPage.cy.js`
- Test: "should be successful with valid credentials"
- Steps:
  1. Enter username from config
  2. Enter password from config
  3. Click Login button
  4. Wait for login API response
  5. Close success notification
  6. Logout

**Clean, straightforward login test!**

---

## Final Test Specification

### What This Test Verifies
User can successfully login with valid credentials and access the application.

### Prerequisites
**Seeded user:**
- Username: [seeded, e.g., "test.user@activeops.com"]
- Password: [seeded]
- Status: Active
- Has role/permissions assigned

### Test Steps
1. Navigate to login page
2. Enter valid username
3. Enter valid password
4. Click Login button
5. Verify: Redirected to dashboard/home page
6. Verify: User successfully authenticated (check URL or dashboard element)

### Expected Results
- Login succeeds
- User redirected to application (not login page)
- Dashboard or default landing page loads
- No error messages displayed

### Automation Approach
- **Pattern:** Pattern A (workflow test)
- **Page Objects:** LoginPage, DashboardPage (or HomePage)
- **Locators:**
  - Username: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })`
  - Password: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })`
  - Login button: `Page.GetByRole(AriaRole.Button, new() { Name = "Login" })`
- **Verification:** 
  - URL no longer contains "/login" or "/"
  - Dashboard element visible (e.g., workgroup selector, nav panel)

---

## Notes
- This is the core smoke test for login functionality
- Should run in every test suite
- Forms basis for authenticated test setup (auth state reuse)
- French translation ignored (not implemented in Cypress)
