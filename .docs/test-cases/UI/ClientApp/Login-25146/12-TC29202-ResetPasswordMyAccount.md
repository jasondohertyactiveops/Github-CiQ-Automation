# TC29202: Reset Password from My Account Page

**Azure Test Case ID:** 29202  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Single step: "My accounts page"
- No details provided

**Cypress Implementation:**
- File: `cypress/e2e/Global/AccountDetails.cy.js`
- Tests in "Update Password" section:
  1. "should have the functionality to update password"
     - Navigate to My Account
     - Click "Change Password" button in Login Details card
     - Verify Change Password dialog appears
     - Test password validation (requirements)
     - Enter current password, new password, confirm
     - Submit
     - Verify success message "Password Successfully Changed"
  2. "should be possible to login with the new password"
     - Logout
     - Login with new password
     - Verify login succeeds

**Clean user self-service workflow!**

---

## Final Test Specification

### What This Test Verifies
User can change their own password from My Account page and login with new credentials.

### Prerequisites

**Seeded user:**
- Username: "passwordchange.user@activeops.com"
- Current password: "CurrentPass@123"
- Status: Active

### Test Steps

**Part 1: Access Password Change Dialog**
1. Login as seeded user
2. Navigate to My Account (via user menu)
3. Click "Change Password" button in Login Details card
4. Verify: Dialog appears with title "Change Your Password"
5. Verify: Dialog shows:
   - Password requirements text
   - Current Password field
   - New Password field
   - Confirm New Password field
   - Cancel and Submit buttons

**Part 2: Password Validation**
1. Test password too short → Verify validation error
2. Test missing uppercase → Verify validation error
3. Test missing symbols → Verify validation error
4. Test password mismatch → Verify error message
5. Test same as current → Verify error message

**Part 3: Successful Password Change**
1. Enter current password: "CurrentPass@123"
2. Enter new password: "NewPassword@456" (meets requirements)
3. Confirm new password: "NewPassword@456"
4. Click Submit
5. Verify: Success message "Password Successfully Changed"
6. Verify: Dialog closes

**Part 4: Login with New Password**
1. Logout
2. Navigate to login page
3. Enter username: "passwordchange.user@activeops.com"
4. Enter NEW password: "NewPassword@456"
5. Click Login
6. Verify: Login successful, dashboard loads

### Expected Results
- Password change dialog accessible and functional
- Password validation enforced (requirements)
- User can successfully change password
- Old password no longer works
- New password works for login

### Automation Approach
- **Pattern:** Pattern A (workflow test)
- **Page Objects:** MyAccountPage, ChangePasswordDialog, LoginPage, DashboardPage
- **Locators:**
  - Change Password button: GetByRole Button
  - Dialog: GetByRole Dialog
  - Password fields: GetByLabel "Current Password", "New Password", "Confirm New Password"
  - Submit: GetByRole Button "Submit"
  - Success message: GetByText or by role Alert
- **Verification:** Dialog appears, validation works, password changes, login succeeds

---

## Notes
- User self-service password change (not admin-initiated like TC29201)
- Tests dialog interaction and form validation
- Verifies password change persists (can login with new password)
- Different from TC25061 (reset via email link) - this is authenticated user changing own password
- Related: TC25061 (reset from login), TC29201 (forced change after admin reset)
