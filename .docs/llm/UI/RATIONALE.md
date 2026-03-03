# Decision Rationale

Why we made key decisions. Reference this for consistency and to inform future choices.

---

## xUnit
Already used at ActiveOps. Modern design, better parallelization, constructor/Dispose pattern. Not worth forcing the team to learn NUnit even though it's more popular in the Playwright community.

## Page Object Model
Essential at scale. When a locator changes, update one page class instead of 50 tests. Start with Pages, add Components as patterns emerge.

## Semantic Locators First
No `data-testid` pollution in production code. Tests what users see, tests accessibility for free, more resilient than CSS classes. Product engineers shouldn't maintain test infrastructure. Hierarchy: GetByRole/GetByLabel/GetByText → CSS selectors → test IDs (last resort only).

## Pre-Seeded Database
Known starting state every run. No test pollution, no setup time, deterministic, parallel-safe. Alternative (create data via UI) is too slow at scale. Alternative (shared DB across runs) causes interference.

## No Database Queries in UI Tests
UI tests verify what the user sees. Database verification is the API test suite's job. This avoids duplication, keeps tests focused, and matches the real user perspective.

## Feature-Based Test Organisation
Tests grouped by feature (`/Tests/Login/`, `/Tests/Admin/`) not by test suite or flat. Developer-friendly ("I changed login, where are those tests?"), scales well, mirrors app structure. Filtering by suite/plan handled via attributes.

## Two Test Patterns
**Pattern A (Workflow):** Dependent steps in sequence — login → navigate → edit → save → verify. **Pattern C (Validation):** Independent checks with shared setup — page has logo, has username field, has password field. Use the right tool for the job.

## No Test IDs in Production Code
Cypress used `elementId` everywhere, creating tension with product engineers. Playwright's semantic locators cover 90%+ of cases without touching production code. Test IDs only as absolute last resort for complex custom widgets.

## Reusable Auth State
Loading saved auth state is instant vs 5-10s per login. At 200+ tests that's 16+ minutes saved. SSO redirects also become a non-issue. Exception: login tests themselves test the login flow directly.

## UI Test Scope
UI tests cover: user workflows, CRUD via UI, navigation, form validation, error messages, grid display. UI tests do NOT cover: background schedulers, database triggers, time-based workflows, performance, security. Those belong in API/integration suites.

---

## When to Revisit
Revisit if: team structure changes significantly, app architecture changes, test suite becomes unmaintainable, or concrete pain points emerge. Don't revisit because someone suggests "we should try X" without a specific problem.
