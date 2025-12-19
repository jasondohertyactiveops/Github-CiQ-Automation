# Project Instructions

## Overview
Automated Playwright .NET tests generated from Azure DevOps manual test cases.

## Goals
- Convert manual test cases from Azure Test Plans into Playwright .NET (C#) tests
- Maintain traceability between automated tests and Azure test cases
- Create reliable, maintainable, readable tests
- Enable iterative generation and healing of tests

---

## Questions & Decisions

### Test Framework & Structure
**Q1: Test framework - NUnit, xUnit, or MSTest?**
- Answer: **xUnit** - Already used at ActiveOps, modern design, better parallelization, cleaner setup/teardown via constructor/Dispose

**Q2: Page Object Model - Use POM (Pages folder with page classes) or simpler direct test approach?**
- Answer: **Yes, use Page Object Model**
  - Essential for hundreds of tests and complex screens
  - Pages represent UI pages/sections as classes
  - Components represent reusable custom controls (grids, widgets)
  - Improves maintainability, readability, and reusability
  - Start with Pages, add Components as patterns emerge

**Q3: Test organization - How should tests be grouped? By Azure Test Suite? By feature? Flat structure?**
- Answer: **By Feature/Module** with xUnit traits for filtering
  - Structure: `/Tests/Login`, `/Tests/CapacityPlanning`, `/Tests/ManageData`, etc.
  - Use `[Trait]` attributes for cross-cutting concerns: `[Trait("Suite", "Smoke")]`, `[Trait("Feature", "Login")]`
  - Enables running: `dotnet test --filter "Suite=Smoke"` or `--filter "Feature=CapacityPlanning"`

### Naming Conventions
**Q4: Test class names - How should they relate to Azure test case IDs?**
- Answer: **Descriptive names without "Tests" suffix**
  - Example: `LoginScreenValidation.cs` (not `LoginScreenValidationTests.cs`)
  - Entire codebase is tests, so "Tests" suffix is redundant
  - Azure Test Case ID referenced in XML summary comment only:
    ```csharp
    /// <summary>
    /// Azure Test Case: 2813
    /// Verify that the Login screen shows as expected
    /// </summary>
    ```
  - Do NOT use `[Trait("TestCase", "2813")]` - creates useless filtering (all IDs unique) 

**Q5: Test method names - Mirror Azure test case titles exactly, or C#-friendly sanitized versions?**
- Answer: **C#-friendly sanitized (PascalCase)**
  - Remove unnecessary words: "Verify that", "Check that", "Ensure"
  - Example: Azure title "Verify that the Login screen shows as expected" → `LoginScreenShowsAsExpected()`
  - Follow .NET naming conventions
  - Still traceable via Azure ID in class comment 

### Azure DevOps Integration
**Q6: Linking - Should test classes/methods include attributes or comments with the Azure test case ID?**
- Answer: **XML summary comment only**
  - Azure Test Case ID and title in class-level XML comment (already decided in Q4)
  - No additional attributes or inline comments needed
  - Traceability maintained via PR links to Azure test case work items
  - Example: PR automating TC2813 links to work item #2813 in Azure DevOps

**Q7: Traceability - Any specific format for linking back to Azure?**
- Answer: **Via Pull Requests**
  - Link PRs to test case work items when automating tests
  - Azure DevOps tracks: "Test case #2813 automated in PR #456"
  - No need for elaborate linking mechanisms in code 

### Authentication & Data
**Q8: Does your app require login? Need reusable auth state?**
- Answer: **Yes, reusable auth state (Option A)**
  - Two authentication methods to test: Username/Password and Microsoft SSO
  - Save auth state once per user type, reuse across all tests (fast, reliable)
  - Create separate auth state files per test user: `auth-admin.json`, `auth-readonly.json`, etc.
  - Seeded test users with known permission profiles in specific workgroups
  - Workgroup switching is a UI action (testable feature)
  - Tests use appropriate auth state based on permissions needed

**Q9: Test data - Hardcoded, config files, or pulled from somewhere?**
- Answer: **Pre-seeded database with known IDs**
  - Fresh database created from seed script before each test run
  - Seed script uses identity insert OFF to set specific, known IDs
  - Tests reference known entities by ID/name from seeded data
  - CRUD tests create NEW entities, functional tests use EXISTING seeded entities
  - **NO database interrogation in UI tests:**
    - UI tests only interact via UI
    - Backend/database verification handled by API test suite
    - No cleanup needed (fresh DB each run)
  - **Configuration via appsettings.json:**
    - `appsettings.json` - Base/shared config
    - `appsettings.Development.json` - Local dev environment
    - `appsettings.Test.json` - Test environment
    - `appsettings.Staging.json` - Staging environment
    - Contains: Base URL, test credentials, environment-specific settings
    - Use standard .NET `ConfigurationBuilder` approach 

### Playwright Specifics
**Q10: Locator preference - `page.GetByRole()` style, `data-testid`, CSS selectors, or mixed approach?**
- Answer: 

**Q11: Execution mode - Headless by default, or headed for debugging?**
- Answer: 

**Q12: Failure artifacts - Screenshots/traces on failure? Video recording?**
- Answer: 

### Application Details
**Q13: App URL - What's the base URL of the SaaS app?**
- Answer: 

**Q14: App technology - Any specific details (SPAs, iframes, shadow DOM, etc.)?**
- Answer: 

### Folder Structure
**Q15: Test organization - Flat `/Tests` folder, or organize by Test Plan/Suite/Feature?**
- Answer: 

**Q16: Additional folders needed - Helpers? Fixtures? Utilities?**
- Answer: 

---

## Test Structure Patterns

### Pattern Selection (Hybrid Approach)
Choose the appropriate pattern based on test type:

**Pattern A: Single Comprehensive Test**
Use for workflow tests with dependent steps.
```csharp
[Fact]
public async Task CanCreateTeamAndAssignStaff()
{
    await Page.GotoAsync("https://app.controliq.com");
    // Create team
    // Assign staff
    // Verify results
    // All steps in sequence, dependent on each other
}
```
- ✅ Use when: Steps depend on previous steps, workflow testing, state matters
- ✅ Example: Login → Navigate → Edit → Save → Verify

**Pattern C: Multiple Tests with Shared Setup**
Use for pure validation tests with independent checks.
```csharp
public class LoginScreenValidation : PlaywrightTest, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await Page.GotoAsync("https://app.controliq.com");
    }
    
    [Fact] public async Task CoreElementsPresent() { /* check elements */ }
    [Fact] public async Task StylingCorrect() { /* check styling */ }
}
```
- ✅ Use when: Pure validation (no state changes), independent assertions, element checking
- ✅ Example: Checking login page has correct elements, styling, behavior
- ⚠️ Note: xUnit runs tests in unpredictable order - use `[Collection("Sequential")]` if order matters

**When in doubt:** Start with Pattern A, refactor to Pattern C if test becomes unwieldy.

## Test Data Strategy

### Pre-Seeded Database Approach
- Fresh database seeded from script before test run
- Known, deterministic starting state
- Tests are independent and can run in any order
- Each feature area tests against pre-seeded data (teams, staff, roles, etc.)
- CRUD operations create NEW entities for testing
- Functional tests use EXISTING pre-seeded entities

**Benefits:**
- ✅ No test pollution or interference
- ✅ Easy debugging (same data every time)
- ✅ Tests can run in parallel
- ✅ Can test specific areas without running setup tests first
- ✅ Example: Test Manage Data without running Teams CRUD tests first

**Out of Scope for UI Testing:**
- Background schedulers and async jobs
- Database triggers and stored procedures
- System integration tests (better suited for API test suite)
- Complex time-based workflows requiring waiting for scheduled tasks

## Final Decisions
**Answered:** Q1, Q2, Q3, Q4, Q5, Q6, Q7, Q8, Q9
**Remaining:** Q10-Q16

---

## Project Structure
(Will be defined after decisions are made)
