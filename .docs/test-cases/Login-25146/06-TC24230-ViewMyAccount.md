# TC24230: View My Account Page

**Azure Test Case ID:** 24230  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Step 1: "My Account page contains correct Personal Information of logged in user"
- Step 2: "Check the translated content in French"

**Cypress Implementation:**
- File: `cypress/e2e/Global/AccountDetails.cy.js`
- Tests:
  1. User menu icon shows initials
  2. User menu dropdown shows name, email, options (My Account, Logout)
  3. My Account page title shows full name
  4. Employee Details card: User type, employment type, ref ID, location
  5. Login Details card: Username displayed, password change button enabled
  6. Password change dialog and workflow
  7. Login with new password after change

**Issues with Cypress:**
- Uses `cy.queryDatabase()` in before/after hooks (should use seeded data)
- French translation: Not implemented (ignored)

---

## Final Test Specification

### What This Test Verifies
User can navigate to My Account page and view their personal information correctly.

### Prerequisites
**Seeded user with known data:**
- Username: [seeded]
- Password: [seeded]
- First/Last name: [seeded]
- Email: [seeded]
- User type: Administrator
- Employment type: Full-time
- Ref ID: [seeded]
- Location: [seeded timezone]

### Test Steps

**Step 1: User Menu Verification**
1. Login as seeded user
2. Verify user icon shows initials (derived from first/last name)
3. Click user icon
4. Verify dropdown shows:
   - Full name (first + middle + last)
   - Email address
   - "My Account" option
   - "Logout" option

**Step 2: My Account Page Navigation**
1. Click "My Account" from dropdown
2. Verify page title shows full name

**Step 3: Employee Details Card**
1. Verify card displays:
   - User Type: Administrator
   - Employment Type: Full-time
   - Ref. ID: [matches seeded data]
   - Location: [matches seeded timezone]

**Step 4: Login Details Card**
1. Verify card displays:
   - Username: [matches seeded username]
   - Password: [hidden/masked]
   - "Change Password" button (enabled)

### Expected Results
- All user information matches seeded data
- Page layout and elements render correctly
- No errors or missing data

### Automation Approach
- **Pattern:** Pattern C (multiple validation tests with shared setup)
- **Page Objects:** 
  - UserMenuComponent (icon, dropdown)
  - MyAccountPage (page structure, cards)
- **Locators:**
  - User icon: By aria-label or data structure
  - Dropdown: By role Menu
  - Cards: By heading/section structure
  - Fields: GetByLabel for each field
- **Shared Setup:** Login once, navigate to My Account, all tests verify different aspects

---

## Notes
- Password change workflow can be separate test (involves dialog interaction, form validation)
- French translation ignored (not implemented in Cypress, localization testing out of scope)
- Use seeded data for user details (no database queries needed)
