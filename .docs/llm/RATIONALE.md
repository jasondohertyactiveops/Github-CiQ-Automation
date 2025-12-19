# Decision Rationale

This document explains **why** we made key architectural and technical decisions. Use this to stay consistent with the project's philosophy and to inform future decisions.

---

## Table of Contents

1. [Why xUnit?](#why-xunit)
2. [Why Page Object Model?](#why-page-object-model)
3. [Why Semantic Locators?](#why-semantic-locators)
4. [Why Pre-Seeded Database?](#why-pre-seeded-database)
5. [Why No Database Interrogation?](#why-no-database-interrogation)
6. [Why Feature-Based Organization?](#why-feature-based-organization)
7. [Why Two Test Patterns?](#why-two-test-patterns)
8. [Why No Test IDs?](#why-no-test-ids)
9. [Why Reusable Auth State?](#why-reusable-auth-state)
10. [What's Out of Scope?](#whats-out-of-scope)

---

## Why xUnit?

**Decision:** Use xUnit as the test framework.

**Context:**
- Already used at ActiveOps
- Choice between NUnit, xUnit, MSTest

**Rationale:**
1. **Team familiarity** - Developers already know xUnit
2. **Modern design** - Built from lessons learned from NUnit/MSTest
3. **Better parallelization** - Runs test classes in parallel by default
4. **Cleaner patterns** - Constructor/Dispose instead of SetUp/TearDown
5. **No static state** - Reduces flakiness

**Alternative considered:** NUnit
- More popular with Playwright community
- But not worth forcing team to learn new framework

**Outcome:** xUnit provides the best balance of team knowledge and technical capability.

---

## Why Page Object Model?

**Decision:** Use Page Object Model (POM) from day one.

**Context:**
- Hundreds of tests to write
- Complex CiQ application
- Multiple developers will maintain tests

**Rationale:**
1. **Maintainability** - When UI changes, update one place, not 50 tests
2. **Reusability** - Common actions (login, navigate, search) defined once
3. **Readability** - Tests read like user actions, not technical selectors
4. **Scalability** - Essential when building hundreds of tests
5. **Collaboration** - Devs can update page objects when they change UI

**Example benefit:**
- Login button locator changes
- Without POM: Update 50 test files
- With POM: Update LoginPage.cs once

**Alternative considered:** Direct Playwright calls in tests
- Simpler initially
- But quickly becomes unmaintainable at scale

**Outcome:** POM is essential for a test suite of this size and complexity.

---

## Why Semantic Locators?

**Decision:** Prioritize semantic locators (GetByRole, GetByLabel, GetByText) over CSS selectors and test IDs.

**Context:**
- Cypress test suite uses `elementId` attributes throughout codebase
- Team doesn't want to pollute codebase with test-specific attributes

**Rationale:**
1. **No code pollution** - Don't add test-specific attributes to production code
2. **Tests what users see** - Locators based on visible text/roles
3. **Tests accessibility** - Proper ARIA roles mean better accessibility
4. **More resilient** - Less fragile than CSS classes that change with styling
5. **Playwright best practice** - Official recommendation
6. **Developer respect** - Product engineers shouldn't maintain test infrastructure

**Hierarchy:**
1. Semantic locators (GetByRole, GetByLabel, GetByText)
2. CSS selectors (for complex components where semantic doesn't work)
3. Test IDs (absolute last resort)

**Example:**
```csharp
// Good - semantic, resilient
await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

// Acceptable - structural when needed
await Page.Locator(".capacity-grid .row[data-id='123']").ClickAsync();

// Avoid - pollutes codebase
await Page.GetByTestId("save-button-element-id").ClickAsync();
```

**Alternative considered:** data-testid everywhere (Cypress approach)
- Very stable locators
- But requires dev team to add test IDs to every element
- Creates maintenance burden on product engineers

**Outcome:** Semantic-first keeps codebase clean and tests resilient.

---

## Why Pre-Seeded Database?

**Decision:** Fresh database seeded from script before each test run.

**Context:**
- CiQ has complex data dependencies (teams, staff, roles, permissions, workgroups)
- Tests need reliable, deterministic data

**Rationale:**
1. **Known starting state** - Every test run starts identically
2. **No test pollution** - Tests can't interfere with each other
3. **Faster tests** - No time spent creating data via UI
4. **Deterministic** - Can reference specific IDs, names, dates
5. **Parallel execution** - Tests don't conflict over shared data
6. **Easy debugging** - Same data every time makes failures reproducible
7. **Independent tests** - Each test has data it needs, doesn't rely on other tests

**Example workflow:**
```
1. Drop database
2. Run seed script (creates teams, staff, roles)
3. Run all tests (use seeded data)
4. Tests pass/fail consistently
```

**Alternative considered:** Create data via UI in each test
- More "true" end-to-end
- But extremely slow (creating 50 teams via UI = 30+ minutes)
- Tests become dependent and fragile

**Alternative considered:** Shared database across runs
- Faster initially
- But tests interfere, data becomes polluted, failures hard to debug

**Outcome:** Pre-seeded database enables fast, reliable, independent tests at scale.

---

## Why No Database Interrogation?

**Decision:** UI tests do NOT query database to verify results.

**Context:**
- Current Cypress tests query database
- Question raised: "Should UI tests check database state?"

**Rationale:**
1. **Separation of concerns** - UI tests test UI, API tests test backend
2. **Avoid duplication** - Backend verification already covered by API test suite
3. **True user perspective** - Users can't see database, only UI
4. **Simpler tests** - No database connection management in UI tests
5. **Clearer failures** - If UI shows wrong data, that's the bug (not DB)

**What UI tests verify:**
- ✅ Does the UI show the correct data?
- ✅ Can users complete workflows?
- ✅ Does clicking "Save" show success message?

**What UI tests DON'T verify:**
- ❌ Is the data actually in the database?
- ❌ Is the verified flag set correctly?
- ❌ Are triggers firing?

**Those are API test responsibilities.**

**Example:**
```csharp
// UI test - check what user sees
[Fact]
public async Task CanSaveTeam()
{
    await teamsPage.FillTeamName("Test Team");
    await teamsPage.ClickSave();
    
    // Verify success message (what user sees)
    await Expect(Page.GetByText("Team saved")).ToBeVisibleAsync();
    
    // NOT this - checking DB directly
    // var team = await database.GetTeam("Test Team");
    // Assert.NotNull(team);
}
```

**Exception:** Test data setup via seeding script uses database (that's different - not part of test assertions).

**Outcome:** UI tests verify UI behavior only, trusting API tests for backend verification.

---

## Why Feature-Based Organization?

**Decision:** Organize tests by feature/module, not by test suite or flat structure.

**Context:**
- Hundreds of tests to organize
- Multiple organization options

**Rationale:**
1. **Developer-friendly** - "I changed capacity planning, where are those tests?"
2. **Scales well** - Can grow to hundreds of tests without chaos
3. **Clear ownership** - Feature teams own tests for their features
4. **Flexible filtering** - Can still run by suite using xUnit traits
5. **Matches app structure** - Mirrors how the application is organized

**Structure:**
```
/Tests
  /Login
  /CapacityPlanning
  /ManageData
  /Teams
```

**Filtering still works:**
```bash
dotnet test --filter "Feature=Login"
dotnet test --filter "Suite=Smoke"
dotnet test --filter "Suite=Regression"
```

**Alternative considered:** By Azure Test Suite
```
/Tests
  /SmokeSuite
  /RegressionSuite
  /Sprint47Suite
```
- Harder to find tests by feature
- Test suites change over time
- Less intuitive for developers

**Alternative considered:** Flat structure
```
/Tests
  LoginScreenValidation.cs
  LoginErrorHandling.cs
  CapacityGridInteractions.cs
  ... (200+ files)
```
- Unmanageable at scale
- No logical grouping

**Outcome:** Feature-based organization balances structure with flexibility.

---

## Why Two Test Patterns?

**Decision:** Use Pattern A for workflows, Pattern C for validation tests (hybrid approach).

**Context:**
- Some tests are pure validation (check elements exist)
- Some tests are workflows with dependent steps

**Rationale:**
1. **Different problems need different solutions**
2. **Pattern C for validation** - Multiple independent checks benefit from granular reporting
3. **Pattern A for workflows** - Dependent steps must run in sequence
4. **Pragmatic over dogmatic** - Use the right tool for the job

**Pattern A (Single Comprehensive Test):**
- Use for: Workflows where steps depend on each other
- Example: Create team → Assign staff → Verify in capacity planning
- Benefit: Clear flow, state matters

**Pattern C (Multiple Tests, Shared Setup):**
- Use for: Independent validation checks
- Example: Login page has logo, username field, password field, button
- Benefit: Granular failures, see multiple issues at once

**Example of wrong pattern:**
- Using Pattern C for "Create → Edit → Delete" workflow
- Tests become dependent, must run in order
- Defeats the purpose of independent tests

**Alternative considered:** Always use Pattern A
- Simpler (one pattern)
- But long validation tests become hard to debug

**Alternative considered:** Always use Pattern C  
- More granular reporting
- But workflows with state become fragile

**Outcome:** Hybrid approach uses the right pattern for each test type.

---

## Why No Test IDs?

**Decision:** Avoid `data-testid` attributes except as absolute last resort.

**Context:**
- Cypress tests use `elementId` attributes extensively
- Team feedback: "We don't want test IDs polluting our codebase"

**Rationale:**
1. **Separation of concerns** - Product code shouldn't know about tests
2. **Maintenance burden** - Dev team maintains test infrastructure
3. **Code pollution** - Hundreds of test-specific attributes clutter JSX
4. **Alternative exists** - Semantic locators work for 90% of cases
5. **Developer respect** - Product engineers focus on features, not test selectors

**When test IDs ARE acceptable:**
- Complex custom widgets with no semantic equivalent
- Dynamic content that changes frequently
- Absolute last resort after trying everything else

**Why Cypress uses test IDs:**
- Cypress doesn't have semantic locators like Playwright
- Was considered best practice in Cypress ecosystem
- But Playwright has better alternatives

**Team pain point:**
- Cypress team: "Add elementId to this button"
- Product team: "Why is my code full of test attributes?"
- Tension between teams

**This approach avoids that tension.**

**Outcome:** Semantic-first strategy respects product engineers and keeps codebase clean.

---

## Why Reusable Auth State?

**Decision:** Save auth state once, reuse across all tests.

**Context:**
- CiQ uses SSO (EntraID, Microsoft, Okta)
- Hundreds of tests need to be authenticated
- Logging in is slow (5-10 seconds)

**Rationale:**
1. **Speed** - Loading auth state is instant vs 5-10s login
2. **Reliability** - No SSO redirects to fail during tests
3. **Simplicity** - Tests start already authenticated
4. **Scale** - Hundreds of tests × 5 seconds = hours saved

**How it works:**
```
1. Login once manually/script
2. Save auth state to auth-admin.json
3. All tests load this file
4. Tests start on dashboard, already authenticated
```

**Alternative considered:** Login in every test
- More "true" end-to-end
- But adds 5-10 seconds per test
- 200 tests × 5 seconds = 16+ minutes just logging in
- SSO redirects can be flaky

**Alternative considered:** Login once per test class
- Middle ground
- But still slow, and classes might run in parallel

**Exception:** Login tests themselves obviously need to test login flow.

**Outcome:** Reusable auth state makes tests fast and reliable at scale.

---

## What's Out of Scope?

**Decision:** UI tests do NOT test certain things.

**Context:**
- Not all Azure test cases are suitable for UI automation
- Some things are better tested elsewhere

**Out of Scope:**

### 1. Background Schedulers & Async Jobs
**Example:** TC26056 - Auto-verification scheduler runs every 5 minutes
**Why:** 
- Requires waiting 5+ minutes
- Tests external scheduler, not UI
- Better tested at integration/API level
**Where it belongs:** API test suite

### 2. Database Triggers & Stored Procedures
**Why:**
- Not a UI concern
- Can't verify via UI
**Where it belongs:** Integration test suite

### 3. Complex Time-Based Workflows
**Example:** "Verify data processing completes overnight"
**Why:**
- Too slow for UI tests
- Not user-facing behavior
**Where it belongs:** Integration/system test suite

### 4. Performance Testing
**Why:**
- UI tests focus on functionality
- Performance needs specialized tools
**Where it belongs:** Separate performance test suite

### 5. Security/Penetration Testing
**Why:**
- Requires specialized tools
- Not UI behavior
**Where it belongs:** Security test suite

**What UI tests DO cover:**
- User workflows via UI
- CRUD operations
- Navigation
- Form validation
- Visual elements
- Integration between UI and backend (via visible results)

**Rationale:**
- Stay focused on what users see and do
- Let specialized test types handle specialized concerns
- Avoid slow, flaky tests

---

## Key Principles Summary

1. **Respect the product engineers** - Don't pollute their code with test infrastructure
2. **Test what users see** - If users can't see it, maybe it's not a UI test
3. **Independent tests** - No shared state, no dependencies between tests
4. **Fast feedback** - Pre-seeded data, reusable auth, parallel execution
5. **Maintainable at scale** - POM, feature organization, clear patterns
6. **Pragmatic over perfect** - Use the right tool for each job
7. **Trust other test layers** - API tests verify backend, UI tests verify UI

---

## When to Revisit Decisions

**Revisit if:**
- Team size/structure changes significantly
- Application architecture changes (e.g., move from React to something else)
- Test suite becomes unmaintainable with current approach
- New Playwright features enable better patterns
- Pain points emerge that decisions don't address

**Don't revisit because:**
- One test is hard to write (find a pattern, don't change architecture)
- Someone suggests "we should try X" without concrete pain point
- It's different from how we've always done it

**How to propose changes:**
- Document the pain point
- Show how current approach fails
- Propose specific alternative
- Discuss tradeoffs
- Update this document if adopted

---

## Questions This Document Should Answer

- "Why don't we use test IDs?" → [Why No Test IDs?](#why-no-test-ids)
- "Why not check the database?" → [Why No Database Interrogation?](#why-no-database-interrogation)
- "Why do we have two test patterns?" → [Why Two Test Patterns?](#why-two-test-patterns)
- "Can I automate this scheduler test?" → [What's Out of Scope?](#whats-out-of-scope)
- "Why pre-seed instead of create data in tests?" → [Why Pre-Seeded Database?](#why-pre-seeded-database)

**If this document doesn't answer a "why" question, add it!**
