# Test Case Documentation Instructions

This document explains how to create comprehensive test case markdown files that serve as the source of truth for automation.

---

## Purpose

Test case MD files combine information from three sources to create a clear, actionable specification:
1. **Azure DevOps Test Cases** - Official requirements
2. **Existing Cypress Code** - What's actually being tested
3. **Playwright MCP Exploration** - Current app behavior

These MD files serve as:
- Source of truth for automation
- Gap analysis (what Cypress tests vs what Azure says)
- Reviewable specs before writing code
- Potential updates to Azure test cases later

---

## Folder Structure

```
.docs/
  test-cases/
    {SuiteName}-{SuiteID}/
      TC{ID}-{DescriptiveName}.md
      TC{ID}-{DescriptiveName}-NOT-UI.md
```

**Example:**
```
.docs/
  test-cases/
    Login-25146/
      TC25057-ValidCredentialsLogin.md
      TC25058-InvalidCredentialsLogin.md
      TC25059-EmailLinkExpiry-NOT-UI.md
      TC25060-TokenRefresh-NOT-UI.md
```

---

## Test Case Template

```markdown
# TC{ID}: {Title}

**Azure Test Case ID:** {ID}
**Suite:** {SuiteName}-{SuiteID}
**Category:** [Pure-UI-Appropriate | Not-Pure-UI-Appropriate | Todo-NeedsReview]
**Reason (if Not-UI):** [TimeBased | BackendAuth | EmailSystem | NFR | BackendBusinessRule | ComplexWorkflow]

---

## Source Analysis

### Azure Test Case
- **Title:** [from Azure]
- **Steps:** [list Azure steps]
- **Expected Results:** [from Azure]
- **Issues:** [vague, incomplete, outdated, etc.]

### Cypress Implementation
- **File:** `path/to/cypress/file.cy.js`
- **Test Name:** "should do something"
- **What It Actually Tests:** [describe what Cypress code does]
- **Issues:** [uses data-tags, database queries, tests backend, etc.]

### Playwright MCP Exploration
- **Page URL:** http://ww7client.localhost/...
- **Elements Found:** [list relevant elements and locators]
- **Interactions:** [what can be tested via UI]

---

## Final Test Specification

### What This Test Should Verify
[Clear description of what UI behavior we're testing]

### Prerequisites
- [Seeded data requirements]
- [User permissions needed]
- [Environment state required]

### Test Steps
1. [Step 1 with expected result]
2. [Step 2 with expected result]
3. [Step 3 with expected result]

### Expected Results
- [What user sees/experiences]
- [Success criteria]

### Automation Approach
- **Pattern:** [Pattern A: Workflow | Pattern C: Validation]
- **Page Objects Needed:** [LoginPage, DashboardPage, etc.]
- **Locators:** [Key elements and how to find them]
- **Waits/Timing:** [Special considerations]

---

## Notes
[Any additional context, edge cases, or considerations]
```

---

## Categories Explained

### Pure-UI-Appropriate ✅
Tests that verify user-facing behavior through the UI:
- Login/logout workflows
- Navigation
- Form validation and CRUD operations
- Page element visibility
- User workflows
- Error message display

**Examples:**
- TC25057: Valid credentials login
- TC25058: Invalid credentials show error
- TC25061: Password reset workflow

---

### Not-Pure-UI-Appropriate ❌
Tests that belong in API/Integration/NFR suites:

**Time-Based:**
- Requires waiting hours/days
- Example: TC25059 (24-hour email link expiry)

**Backend Auth:**
- Tests authentication mechanisms
- Token management
- Example: TC25060 (token refresh logic)

**Email System:**
- Tests email delivery
- Email templates
- Link generation

**NFR (Non-Functional Requirements):**
- Performance testing
- Load testing
- Security testing

**Backend Business Rules:**
- Database constraints
- Business logic validation
- Example: TC24155 (username uniqueness after deletion)

**Complex System Integration:**
- Multiple systems coordinating
- Background jobs/schedulers
- Cross-service workflows

---

### Todo-NeedsReview ⚠️
Tests that require discussion:

**ComplexWorkflow:**
- Mix of UI and backend concerns
- Support App involvement
- System configuration changes
- Example: TC24154 (users without email + system config)

**Unclear Scope:**
- Vague test case descriptions
- No clear UI verification point
- Might be better suited elsewhere

---

## How to Create a Test Case MD File

### Step 1: Fetch Azure Test Case
```
Use Azure DevOps MCP to get the test case details
Document: ID, title, steps, expected results
Note any issues with the Azure test case (vague, incomplete, etc.)
```

### Step 2: Find Corresponding Cypress Code
```
Search Cypress codebase for related test
Document: file path, test name, what it actually does
Note: data-tags used, database queries, what's really being tested
Identify any issues (testing wrong layer, etc.)
```

### Step 3: Explore with Playwright MCP
```
Navigate to relevant page
Identify elements (without data-tags)
Test interactions
Document: semantic locators, page behavior, current state of app
```

### Step 4: Synthesize and Categorize
```
Compare all three sources
Identify gaps and conflicts
Decide: Pure UI appropriate or not?
If Not Pure UI: Document reason clearly
If Pure UI: Write clear specification for automation
If unclear: Mark as Todo-NeedsReview with questions
```

### Step 5: Write MD File
```
Follow template format
Be specific and actionable
Include all source references
Document locators and approaches
```

---

## Example Workflow

**Creating TC25057-ValidCredentialsLogin.md:**

1. **Fetch from Azure:**
   - Title: "User with valid credentials is able to login"
   - Steps: Vague, just says "User is able to activate... and login"
   - Issue: Not detailed enough

2. **Check Cypress:**
   - File: `cypress/e2e/Global/LoginPage.cy.js`
   - Test: "should be successful with valid credentials"
   - Actually tests:
     ```javascript
     cy.get('[data-tag="username"]').type(username);
     cy.get('[data-tag="password"]').type(password);
     cy.get('[data-tag="submit-btn"]').click();
     cy.wait("@postLogin");
     cy.Logout();
     ```
   - Issues: Uses data-tags, but otherwise good UI test

3. **Explore with PW MCP:**
   - Navigate to http://ww7client.localhost/
   - Find: Username field → `Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" })`
   - Find: Password field → `Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" })`
   - Find: Login button → `Page.GetByRole(AriaRole.Button, new() { Name = "Login" })`
   - Interaction works as expected

4. **Synthesize:**
   - Category: Pure-UI-Appropriate ✅
   - Test: Login workflow with valid credentials
   - Clear UI behavior to verify
   - Can use semantic locators (no data-tags needed)

5. **Write MD file** with complete specification

---

## Quality Checklist

Before finalizing a test case MD file:

- [ ] Category is accurate (UI vs Not-UI vs Todo)
- [ ] If Not-UI, reason is clearly documented
- [ ] Azure test case details captured
- [ ] Cypress implementation analyzed and referenced
- [ ] Current app explored with PW MCP
- [ ] All three sources reconciled
- [ ] Conflicts/gaps identified
- [ ] Steps are clear and actionable
- [ ] Prerequisites identified
- [ ] Expected results defined
- [ ] Locators documented (semantic, no data-tags)
- [ ] Pattern choice justified (A vs C)
- [ ] Any special considerations noted

---

## When Sources Conflict

**Priority order:**
1. **Current app reality** (PW MCP) - What actually exists NOW
2. **Cypress code** - What's actually being tested (even if wrong layer)
3. **Azure test case** - Original intent (often vague or outdated)

**Example conflict:**
- **Azure:** "Check login page shows correctly"
- **Cypress:** Tests database queries, French translation, AND login
- **PW MCP:** Shows current page structure, no "Remember me" checkbox

**Resolution in MD file:**
```markdown
## Source Conflicts

**Azure says:** Check login page elements
**Cypress does:** Tests login + database + translation (over-scoped)
**Current app has:** No "Remember me" checkbox (removed?)

**Decision:** Test login functionality only (ignore database/translation)
Use current app elements (no Remember me checkbox)
```

---

## Tips for Success

1. **Be honest about conflicts** - Don't paper over gaps between sources
2. **Question everything** - Just because Cypress tests it doesn't mean it's right
3. **Focus on UI behavior** - If a user can't see/do it, it's not a UI test
4. **Document unknowns** - Use Todo category liberally
5. **Reference all sources** - Makes review and updates easier
6. **Be specific about locators** - Future automation depends on this

---

## File Naming Conventions

**UI-Appropriate tests:**
- `TC{ID}-{DescriptiveName}.md`
- Example: `TC25057-ValidCredentialsLogin.md`

**Not-UI tests:**
- `TC{ID}-{DescriptiveName}-NOT-UI.md`
- Example: `TC25059-EmailLinkExpiry-NOT-UI.md`

**Todo/Unclear:**
- `TC{ID}-{DescriptiveName}-TODO.md`
- Example: `TC24154-UsersWithoutEmail-TODO.md`

This helps quickly identify test status by filename alone.
