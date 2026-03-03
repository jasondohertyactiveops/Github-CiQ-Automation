# TC29201: Reset Password from Staff Member Setup

**Azure Test Case ID:** 29201  
**Suite:** Login-25146  
**Plan:** Smoke, Regression  

---

## Source Analysis

**Azure Test Case:**
- Single step: "User set up screen"
- Vague, no details

**Cypress Implementation:**
- File: `cypress/e2e/UserJourney/UserWithEmailNotRequired.cy.js`
- Test: "should be redirected to change password screen if another user resets it from staffmember setup"
- What it does:
  1. Admin logs in
  2. Navigates to Staff Members page
  3. Edits target user
  4. Sets new password in edit form
  5. User logs in with reset password
  6. Gets 403 response
  7. Redirected to Change Password screen (forced change)

**What this actually tests:**
- Admin can reset user password from Staff Members setup (Admin feature)
- User is forced to change password on next login (Login behavior)

---

## Final Test Specification

### What This Test Verifies
User is forced to change password after admin reset and can successfully complete the workflow.

### Prerequisites

**Seeded user with forced password change:**
- Username: "mustchange.user@activeops.com"
- Password: "TempPassword@123"
- Status: Active
- Flag: MustChangePasswordOnNextLogin = true (or equivalent)

This simulates an admin having reset the user's password.

### Test Steps

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | Login with temp password, verify redirect to /changepassword, change password, login with new password, verify redirect to /rtm, logout and re-login to confirm flag cleared | Verify |

### Expected Results
- User with "must change" flag cannot access app without changing password
- Change password workflow completes successfully
- User can login with new password
- Subsequent logins don't trigger password change

### Automation Approach
- **Pattern:** Pattern A (workflow test)
- **Page Objects:** LoginPage, ChangePasswordPage, DashboardPage
- **Locators:**
  - Login fields: GetByRole Textbox
  - Current password: GetByLabel "Current Password"
  - New password: GetByLabel "New Password"
  - Confirm: GetByLabel "Confirm New Password"
  - Submit: GetByRole Button "Submit"
- **Verification:** URL changes, password change screen appears, final login succeeds

---

## Notes
- Tests forced password change workflow (triggered by admin reset)
- Admin reset action itself belongs in Admin/User Management tests
- This test focuses on user-facing behavior after admin action
- Seeded flag simulates admin having performed reset
- Related: TC25061 (reset from login), TC29202 (reset from My Account)
