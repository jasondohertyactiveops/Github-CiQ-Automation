# TC25058: Invalid Credentials Login

**Azure Test Case ID:** 25058  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Step 1: Incorrect Username
- Step 2: Incorrect Password
- Step 3: No permissions assigned
- Step 4: Inactive account
- Step 5: Check French validation messages

**Cypress Implementation:**
- File: `cypress/e2e/Global/LoginPage.cy.js`
- Tests:
  - "should throw an error when no role is assigned" (uses database query - should use seeded data)
  - "should unsuccessful with invalid username and password" (tests error message)
  - Other tests cover field validation (empty username, empty password, clear fields)

**Cypress covers:**
- Invalid credentials error message
- No role assigned error (but uses `cy.queryDatabase()`)
- Field validation errors

**Missing in Cypress:**
- Inactive account scenario

---

## Final Test Specification

### What This Test Verifies
Login fails appropriately with different types of invalid credentials and shows correct error messages.

### Prerequisites

**Seeded users:**
1. **Invalid credentials test:** Any non-existent username/password combo
2. **No role user:**
   - Username: "norole.user@activeops.com"
   - Password: [seeded]
   - Status: Active
   - Roles: None assigned
3. **Inactive user:**
   - Username: "inactive.user@activeops.com"
   - Password: [seeded]
   - Status: Inactive/Disabled

### Test Steps

**Scenario 1: Invalid Username/Password**
1. Navigate to login page
2. Enter invalid username
3. Enter any password
4. Click Login
5. Verify: Error message "Sorry, your login was unsuccessful. Please check your login credentials, or follow the reset password link"
6. Verify: Fields not cleared, login button still enabled

**Scenario 2: No Role Assigned**
1. Navigate to login page
2. Enter username: "norole.user@activeops.com"
3. Enter valid password
4. Click Login
5. Verify: Error message "You don't have any roles assigned, please contact your administrator."
6. Verify: User not logged in

**Scenario 3: Inactive Account**
1. Navigate to login page
2. Enter username: "inactive.user@activeops.com"
3. Enter valid password
4. Click Login
5. Verify: Error message [TBD - need to determine actual message]
6. Verify: User not logged in

### Expected Results
- Each scenario shows appropriate error message
- User remains on login page
- No partial authentication occurs
- Error messages are clear and actionable

### Automation Approach
- **Pattern:** Pattern C (multiple scenarios, shared page setup)
- **Page Objects:** LoginPage
- **Locators:**
  - Username: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })`
  - Password: `Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })`
  - Login button: `Page.GetByRole(AriaRole.Button, new() { Name = "Login" })`
  - Error message: `Page.GetByText()` or by role Alert
- **Verification:**
  - Error message text matches expected
  - URL still on login page
  - User not authenticated

---

## Notes
- Inactive account error message needs verification with PW MCP
- French translation ignored (not implemented in Cypress)
- Seeded data required for no-role and inactive users
- May split into separate test methods per scenario for clarity
