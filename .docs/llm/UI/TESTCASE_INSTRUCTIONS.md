# UI Test Case Documentation

Test case MD files are the source of truth for automation, combining Azure DevOps test cases, Cypress legacy code, and current app behavior.

---

## Folder Structure
```
.docs/test-cases/UI/ClientApp/{SuiteName}-{SuiteID}/
  {Order}-TC{ID}-{DescriptiveName}.md
  {Order}-TC{ID}-{DescriptiveName}-NOT-UI.md
  {Order}-TC{ID}-{DescriptiveName}-TODO.md
```

## Template

```markdown
# TC{ID}: {Title}

**Azure Test Case ID:** {ID}  
**Suite:** {SuiteName}-{SuiteID}  
**Plan:** Smoke, Regression  

---

## Source Analysis

**Azure Test Case:** {Steps and expected results from Azure}
**Cypress:** {File path, what it tests, issues}
**Current App:** {What Playwright MCP found}

---

## What This Tests

{1-2 sentence description}

---

## Test Steps

### Setup (Fixture/Test Body)
- {Describe arrange/act setup}

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | {What this step verifies} | Verify |
| 2 | {What this step verifies} | Verify |

---

## Notes
{Important context, edge cases}
```

## Step Table Rules

Every step with `Verify` coverage must map to an `[AzureTestStep(tcId, stepNumber)]` in code. Steps that are arrange/act (navigate, fill form, submit) are documented as Setup in the fixture/test body section, not numbered as Verify steps.

## Categories

**Pure-UI-Appropriate:** Tests user-visible behavior (login, navigation, form validation, CRUD workflows, error messages, grid display). UI tests are shallow — verify what the user sees, no database checks.

**Not-Pure-UI (suffix -NOT-UI):** Time-based, backend auth, email system, NFR, backend business rules. Document reason in file.

**Needs Review (suffix -TODO):** Unclear scope, complex workflow, needs discussion.

## Source Priority When Conflicts Exist
1. **Current app** (Playwright MCP) — what actually exists now
2. **Cypress code** — what's actually being tested
3. **Azure test case** — original intent (often vague/outdated)
