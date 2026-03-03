# API Test Case Documentation

Test case MD files specify what to test and which database tables to verify. They are specifications, not implementation guides.

---

## Folder Structure
```
.docs/test-cases/API/ClientAPI/{SuiteName}-{SuiteID}/
  {Order}-TC{ID}-{DescriptiveName}.md
```

## Template

```markdown
# TC{ID}: {Title}

**Azure Test Case:** {ID}  
**Suite:** {SuiteName}-{SuiteID}  
**Plan:** Smoke, Regression  
**Thunderclient:** `collections/{path}` OR ❌ Not implemented  
**Test Users:** {Range, e.g., "9200-9299 range (active user with roles)"}

---

## What This Tests

{1-2 sentence description of backend behavior and database state being verified}

---

## Test Steps

### Setup (Fixture)
- Method: POST/GET/PUT/DELETE
- Endpoint: `/api/path`
- Body: `{ key fields }`

### Response Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 1 | Status code is 200 | Verify |
| 2 | Response contains token | Verify |

### Database Checks

| Step | Description | Coverage |
|------|-------------|----------|
| 3 | Login record created in [UserLoginDetail] | Verify |
| 4 | RefreshToken in DB matches response | Verify |

---

## Notes
{Important context, gaps from Thunderclient}
```

## Step Table Rules

Every step with `Verify` coverage must map to an `[AzureTestStep(tcId, stepNumber)]` in code. Setup steps (API calls, DB queries for fixture) are described in the Setup section, not numbered as Verify steps.

## Key Principle

API tests are DEEP tests. The value-add over UI tests is **database verification**. Every state-changing API test must specify which tables/fields to check for persistence.

## What to Include
- What needs to be tested (brief)
- Thunderclient location (if exists)
- Which database tables to verify
- Gap analysis (what's missing from Thunderclient)

## What to Omit
- Detailed Arrange-Act-Assert implementation
- Specific code examples
- Tool choices and rationale
- Quality checklists
