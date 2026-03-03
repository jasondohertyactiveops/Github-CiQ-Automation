# API Test Project

## Tech Stack
- **.NET 10** / **C#** / **xUnit** / **Dapper** (database queries)
- HTTP via ApiHelper, database verification via shared SqlConnection
- Shared utilities in `AO.Automation.Shared` (TokenHelper, config, attributes)

## Project Structure
```
src/AO.Automation.API.Client/
  Config/             ApiTestConfig (appsettings reader)
  Helpers/            ApiHelper (HTTP client), ApiTestFixture (base class)
  Models/
    Requests/         Request body DTOs by feature
    Responses/        Response body DTOs by feature
    Database/         Database record DTOs
  Tests/              Test classes by feature
    Login/            Login suite tests
    Conventions/      Convention enforcement tests
```

## Key Decisions

**HTTP Client:** ApiHelper wraps HTTP calls with typed responses. Fixture pattern manages lifecycle — one ApiHelper per test class.

**Database:** Dapper on shared SqlConnection (read-only, MARS enabled). Every state-changing API call verifies database state. Use `[dbo].[TableName]` format (no database prefix). Always check `Tables/[TableName].sql` for actual schema before writing queries.

**Models:** Strongly-typed DTOs with `[JsonPropertyName]` attributes. Organised by feature under `Models/Requests/`, `Models/Responses/`, `Models/Database/`.

**Auth:** TokenHelper generates JWT tokens at runtime (no expiry issues). Login tests test auth explicitly; other tests use pre-generated tokens.

**Patterns:**
- **Fixture + Multiple Facts:** One API call in fixture, many focused assertions. Use for: comprehensive response + DB validation (e.g., TC25057 with 12 checks).
- **Theory + InlineData:** Same test logic with different inputs. Use for: negative testing, multiple error scenarios (e.g., TC25058 with 3 invalid login types).

## Traceability Attributes

Class level (this order):
```csharp
[AzureTestSuite(25146)] // Login
[AzureTestCase(25057)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
```

Method level:
```csharp
[Fact]
[AzureTestStep(25057, 1)]
[Trait("Category", "OneShot")]  // method level only, if applicable
```

Convention tests in `Tests/Conventions/` enforce all of these — missing attributes fail the build.

## Test Execution

```powershell
# Full suite
cd src/AO.Automation.API.Client
dotnet test

# Skip OneShot during development
dotnet test --filter "Category!=OneShot"

# By plan, suite, or test case
dotnet test --filter "Plan=Smoke"
dotnet test --filter "Suite=25146"
dotnet test --filter "TC=25057"

# Convention tests only
dotnet test --filter "Category=Convention"
```

## Test Users
- **9200-9299:** API test users (separate from UI to avoid conflicts)
- All use password `Workware@1` unless noted
- See `SEEDING-REFERENCE.md` for complete registry

## Local Environment
- API: http://localhost:8080
- SQL: localhost,1434 (sa / yourStrong(!)Password)
- Token config: `WW7/ww7-api/AO.WW/AO.WW.Web.Api.Client/appsettings.Containers.json`
  - access-token-expiry: 30 min
  - ww7client-timeout-general: 90 min (refresh token)

## Finding Schema & API Details
- **Database schema:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Tables/`
- **Swagger:** Uploaded swagger.json or runtime endpoint
- **Thunderclient:** `WW7/Projects/ControliQAutomation/` (reference only, being replaced)

## UI vs API Test Differences

| Aspect | UI Tests | API Tests |
|--------|----------|----------|
| Technology | Playwright Browser | ApiHelper (HTTP) + Dapper (DB) |
| What's tested | User-visible behaviour | Backend logic + data persistence |
| Verification | UI elements, workflows | Response + database state |
| Speed | Slower (browser startup) | Faster (HTTP only) |
| Auth | Browser login + saved state | Token generation via TokenHelper |
| Test users | 9000-9199 | 9200-9299 |
| DB queries | Never | Always for state changes |

## Related Docs
- `PATTERNS.md` — Code examples and reference implementations
- `TESTCASE_INSTRUCTIONS.md` — Test case documentation format
- `Shared/WORKFLOW-QUICK.md` — Test generation workflow
- `Shared/DATA-RULES.md` — Test data self-sufficiency rules
