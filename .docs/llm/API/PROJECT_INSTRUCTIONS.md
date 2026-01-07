# API Test Automation - Project Instructions

## Overview
Automated Playwright .NET API tests for ControliQ REST APIs, with database verification.

## Goals
- Test API endpoints directly (faster, more reliable than UI for backend logic)
- Verify database state changes (POST creates records, DELETE removes them, etc.)
- Complement UI tests by testing the backend layer
- Migrate existing Thunderclient tests to proper automation
- Build comprehensive API coverage (Thunderclient has gaps)

---

## Questions & Decisions

### Test Framework & Structure

**Q1: Test framework - NUnit, xUnit, or MSTest?**
- Answer: **xUnit** - Same as UI tests for consistency

**Q2: Project structure - Separate project or combine with UI tests?**
- Answer: **Separate project: AO.Automation.API.Client**
  - Different concerns (HTTP requests vs browser automation)
  - Different dependencies (no Playwright browser binaries needed)
  - Can run independently or together
  - Clear separation of test types

**Q3: Test organization - By API domain? By feature? By endpoint?**
- Answer: **By Feature/Module** matching UI test structure
  - Structure: `/Tests/Login`, `/Tests/Users`, `/Tests/Workgroups`, etc.
  - Use `[Trait]` attributes for filtering:
    - `[Trait("Suite", "Login-25146")]` - Links to Azure Test Suite
    - `[Trait("Feature", "Login")]` - Feature area
    - `[Trait("API", "ClientAPI")]` - Which API (ClientAPI, InternalAPI, etc.)

**Q4: How to handle multiple APIs (ClientAPI, InternalAPI, IntegrationAPI)?**
- Answer: **Single project with organization by API domain**
  - All in AO.Automation.API.Client project
  - Tests organized by feature, tagged by API domain
  - Helpers/Config can be shared across API domains
  - Filter by trait: `--filter "API=ClientAPI"`

### HTTP Client & Requests

**Q5: HTTP client - HttpClient, RestSharp, or Playwright API context?**
- Answer: **Playwright API Context** (primary), HttpClient (fallback)
  - Playwright API context integrates well with test framework
  - Built-in request/response logging
  - Can share authentication state
  - Fallback to HttpClient for scenarios not supported

**Q6: Request/Response models - Create DTOs or use dynamic JSON?**
- Answer: **Create DTOs (strongly-typed models)**
  - `/Models/Requests/` - Request body classes
  - `/Models/Responses/` - Response body classes
  - `/Models/Database/` - Database record classes
  - Enables type-safe testing and IntelliSense
  - Easier to maintain than dynamic JSON

### Authentication

**Q7: How to authenticate API requests?**
- Answer: **Generate tokens programmatically via TokenHelper**
  - Reuse existing TokenHelper from UI project
  - Generate JWT tokens at test runtime (no expiry issues)
  - Store token per test or test class as needed
  - Can also test login endpoint to get real tokens

**Q8: When to test authentication vs when to bypass it?**
- Answer: **Test auth explicitly, bypass for other tests**
  - Dedicated auth tests: Login, token refresh, expiry
  - Other tests: Use pre-generated token (faster)
  - Similar to UI tests using saved auth state

### Database Verification

**Q9: Database access - Direct SQL, Dapper, Entity Framework?**
- Answer: **Dapper for database queries**
  - Lightweight, fast, simple
  - No ORM overhead
  - Raw SQL queries directly on shared SqlConnection
  - Use Dapper extension methods: `QuerySingleOrDefaultAsync`, `QueryAsync`, `ExecuteScalarAsync`
  - Connection managed by ApiTestFixture base class

**Q10: When to verify database state?**
- Answer: **Always for state-changing operations:**
  - POST requests → Verify record created
  - PUT/PATCH requests → Verify record updated
  - DELETE requests → Verify record removed
  - Authentication → Verify login/session records
  - Optional for GET requests (verify data consistency)

**Q11: Database connection - Share across tests or per-test?**
- Answer: **Shared connection via ApiTestFixture**
  - Fixture opens connection once in InitializeAsync
  - All test methods use same connection
  - Proper disposal handled by base class
  - Pattern: Use Dapper directly on `DbConnection` property
  - ApplicationIntent=ReadOnly for safety
  - MultipleActiveResultSets=True (MARS) enabled

### Test Data

**Q12: Test users - Same as UI (9000-9199) or separate range?**
- Answer: **Separate range: 9200-9299**
  - Avoids conflicts if UI and API tests run in parallel
  - Clear distinction in database
  - Same seeding strategy (pre-seeded with known IDs)

**Q13: Test data - Create via API or use pre-seeded?**
- Answer: **Pre-seeded for most tests, create new for CRUD tests**
  - Authentication tests: Use pre-seeded users (9200-9299)
  - Read operations: Use pre-seeded data
  - Create/Update/Delete: Create new entities, verify DB state
  - Same strategy as UI tests

### Validation & Assertions

**Q14: Assertions - xUnit Assert or FluentAssertions?**
- Answer: **xUnit Assert only** (same as UI tests)
  - Consistency across all test projects
  - Standard .NET assertions
  - No additional dependencies

**Q15: Response validation - Status codes only or full schema validation?**
- Answer: **Status codes + key field validation**
  - Always check status code
  - Validate presence and type of critical fields
  - Don't need exhaustive schema validation (that's contract testing)
  - Focus on fields relevant to the test

### Thunderclient Migration

**Q16: How to handle Thunderclient tests?**
- Answer: **Document first, then implement**
  - Create test case MD from Thunderclient collection
  - Add database verification (not in Thunderclient)
  - Implement in Playwright with proper assertions
  - Thunderclient has gaps - build comprehensive coverage

---

## Finding Schema and API Details

### Database Schema Location

**Table definitions:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/`

**ALWAYS check actual schema before writing SQL:**
- Column names (e.g., `Created` not `LoginDateTime`)
- Column types (e.g., `BIGINT` for Id, `NVARCHAR(64)` for RefreshToken)
- Nullable columns
- Constraints and indexes

**Example:** Before querying UserLoginDetail, check:
```
WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/UserLoginDetail.sql
```

**Database queries:**
- Use `[dbo].[TableName]` format (no database name prefix)
- Connection string already specifies database
- Example: `SELECT * FROM [dbo].[UserLoginDetail]` ✅
- NOT: `SELECT * FROM [WW7Client].[dbo].[UserLoginDetail]` ❌

### API Contract Location

**Swagger definition:** Available at uploaded swagger.json or runtime endpoint

**ALWAYS check Swagger for:**
- Actual endpoint paths
- Request body structure
- Response body structure (don't guess!)
- Required vs optional fields

**Example:** Login response is NOT a nested User object - it's flat with firstName, lastName, username, etc.

### Local Environment Configuration

**API Base URL:** `http://localhost:8080`
**SQL Server:** `localhost,1434` (note the comma and port)
**SQL Credentials:** sa / yourStrong(!)Password

**Token Configuration:** `WW7/ww7-api/AO.WW/AO.WW.Web.Api.Client/appsettings.Containers.json`
- `ww7client-timeout-general`: 30 (refresh token expiry for ww7client)
- `refresh-token-expiry`: 90 (default fallback)
- `access-token-expiry`: 30 (JWT token expiry)

---

## Key Principles

1. **Database verification is essential** - API tests verify both response AND database state
2. **Separate test users** - API users (9200-9299) separate from UI users (9000-9199)
3. **Pre-seeded data** - Fresh database with known test data
4. **TokenHelper for auth** - Generate tokens programmatically for speed
5. **Test auth explicitly** - Dedicated tests for login/token operations
6. **Document gaps** - Thunderclient is incomplete, build comprehensive coverage
7. **Simple assertions** - xUnit Assert only, no FluentAssertions
8. **Fixture pattern** - ApiTestFixture base class manages ApiHelper and DbConnection
9. **OneShot tests** - Mark with `[Trait("Category", "OneShot")]` for tests that modify state permanently

---

## OneShot Tests

**What are OneShot tests?**

Tests that modify user/data state permanently and cannot be re-run without resetting the database.

**Examples:**
- Account activation (user goes from Invited to Active - can't activate twice)
- Password changes (old password no longer works)
- Account lockout (failed login counter increments)
- Data deletion (can't delete same record twice)

**How to mark:**
```csharp
[Trait("Category", "OneShot")]
public class AccountActivationApi : IClassFixture<AccountActivationApiFixture>
{
    // Test activates user 9205 - requires fresh database to re-run
}
```

**Running tests:**
```powershell
# Skip OneShot during development iterations
dotnet test --filter "Category!=OneShot"

# Run full suite (requires fresh DB after)
dotnet test
```

**After running OneShot tests:**
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

---

## Current Project State

### Test Coverage
**Login Suite (Login-25146): 0/4 implemented (75% missing from Thunderclient)**
- TC25057: Valid Login (Thunderclient ✅ but needs enhancement)
- TC25058: Invalid Login (Thunderclient ❌ - critical gap)
- TC25060: Token Refresh (Thunderclient ❌ - critical gap)
- TC25059: Email Link Expiry (Azure only, time-based testing)

### Documentation Complete
- Test case template created
- 4 example test cases documented
- Gap analysis showing Thunderclient coverage
- User seeding range allocated (9200-9299)

### Next Steps
1. Create AO.Automation.API.Client project structure
2. Implement DatabaseHelper and ApiHelper
3. Create request/response models
4. Implement first test (TC25058 - Invalid Login, Priority 1)
5. Expand coverage to other API domains

---

## Test Execution

### Commands
**Full suite:**
```powershell
cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation.API.Client
dotnet test
```

**By feature:**
```powershell
dotnet test --filter "Feature=Login"
```

**By API domain:**
```powershell
dotnet test --filter "API=ClientAPI"
```

### Database Recreation
Same as UI tests - requires fresh database:
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

---

## Differences from UI Tests

| Aspect | UI Tests | API Tests |
|--------|----------|-----------|
| **Technology** | Playwright Browser | Playwright API Context |
| **What's Tested** | User-visible behavior | Backend logic & data |
| **Verification** | UI elements, workflows | Response + Database |
| **Speed** | Slower (browser startup) | Faster (HTTP only) |
| **Auth** | Browser login + saved state | Token generation |
| **Test Users** | 9000-9199 | 9200-9299 |
| **Dependencies** | Browser binaries | None (HTTP only) |

---

## Additional Documentation
- **WORKFLOW.md** - How to generate API test cases from Thunderclient
- **PATTERNS.md** - Code examples for API tests
- **TESTCASE_INSTRUCTIONS.md** - Test case documentation format
