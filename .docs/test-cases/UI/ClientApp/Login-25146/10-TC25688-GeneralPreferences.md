# TC25688: View/Update General Preferences

**Azure Test Case ID:** 25688  
**Suite:** Login-25146  
**Category:** ✅ Pure-UI-Appropriate  

---

## Source Analysis

**Azure Test Case:**
- Single step: "If the language feature is enabled with the required permission assigned and user access to more than one language... It should be possible to select change the language preference and see the content translated accordingly"

**Cypress Implementation:**
- File: `cypress/e2e/Global/GeneralPreferences.cy.js`
- Test: "Update language to French and Check the content"
- What it does:
  1. Navigate to My Account
  2. Click General Preferences tab
  3. Change language dropdown to "French (Canada)"
  4. Verify labels on My Account page update to French:
     - Tab names (Account Details, General Preferences)
     - Card titles (Employee Details, Login Details)
     - Field labels (User Type, Username, Password, Location, etc.)
  5. Change back to English
- Uses `translations.json` fixture for expected French text
- **Spot check approach:** Tests one page to verify feature works, not comprehensive coverage

---

## Final Test Specification

### What This Test Verifies
User can change language preference and UI labels update accordingly.

### Prerequisites

**Seeded user:**
- Username: [seeded]
- Password: [seeded]
- Permissions: Access to language preference feature

**Application:**
- Language feature enabled
- French language available as option

### Test Steps

1. Login as seeded user
2. Navigate to My Account
3. Click General Preferences tab
4. Verify: Language dropdown exists
5. Select "French (Canada)" from language dropdown
6. Wait for preference to save
7. Verify: Labels on My Account page change to French (spot check):
   - Tab: "Détails du compte" (Account Details)
   - Tab: "Préférences générales" (General Preferences)
   - Card: "Détails de l'employé" (Employee Details)
   - Card: "Informations de connexion" (Login Details)
8. Change language back to "English (United Kingdom)"
9. Verify: Labels return to English

### Expected Results
- Language dropdown accessible and functional
- Selecting French updates UI labels to French
- Selecting English updates UI labels back to English
- Preference persists (saved to backend)

### Automation Approach
- **Pattern:** Pattern A (workflow test with state change)
- **Page Objects:** MyAccountPage, GeneralPreferencesTab
- **Locators:**
  - General Preferences tab: GetByRole Tab
  - Language dropdown: GetByLabel or by role Combobox
  - Labels to verify: GetByText for specific translations
- **Verification:** 
  - UI labels match expected language
  - Can use simple spot check (3-4 labels sufficient to prove feature works)

---

## Notes
- This tests the language switching FEATURE, not comprehensive translation coverage
- Spot check on My Account page is sufficient to verify feature works
- Does not test every page/label in French (that's localization QA, not functional testing)
- Requires translations reference (expected French text for assertions)
