# UI Test Project

## Tech Stack
- **.NET 10** / **C#** / **xUnit** / **Playwright for .NET**
- Page Object Model in `Pages/`, reusable components in `Components/`
- Shared utilities in `AO.Automation.Shared` (TokenHelper, config, attributes)

## Project Structure
```
src/AO.Automation.UI.Client/
  BaseClasses/        PlaywrightTest, BrowserFixture
  Config/             TestConfig (appsettings reader)
  Pages/              Page Object Model classes
    Login/            LoginPage, ActivationPage, ResetPasswordPage
    MyAccount/        MyAccountPage, ChangePasswordDialog, GeneralPreferencesTab
    Shared/           UserMenuComponent
  Components/         Reusable UI components (grids, widgets)
  Tests/              Test classes by feature
    Login/            Login suite tests
    Admin/            Admin area tests
    Conventions/      Convention enforcement tests
```

## Key Decisions

**Locators:** Semantic first (GetByRole, GetByLabel, GetByText), CSS second, test IDs last resort. No `data-testid` pollution in production code.

**Auth:** Reusable auth state via BrowserFixture. Each test gets isolated Context and Page. Login tests test login directly; everything else starts authenticated.

**Data:** Pre-seeded database with known users/data. UI tests don't query the database — that's the API test suite's job. UI tests verify what the user sees.

**Patterns:**
- **Pattern A (Workflow):** Single test method with dependent steps. Use for: login → navigate → edit → save → verify.
- **Pattern C (Validation):** Multiple test methods with shared fixture setup. Use for: independent checks on the same page.

**Out of scope for UI tests:** Background schedulers, database triggers, time-based workflows, performance testing, security testing. These belong in API/integration suites.

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
# Full suite (requires fresh DB)
cd src/AO.Automation.UI.Client
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
- **9000-9099:** OneShot users (single execution)
- **9100-9199:** Repeatable users (can run multiple times)
- All use password `Workware@1` unless noted
- See `SEEDING-REFERENCE.md` for complete registry

## Local Environment
- Client: http://ww7client.localhost
- Headless by default, headed for debugging
- Screenshots + traces on failure, videos on demand

## Related Docs
- `PATTERNS.md` — Code examples and reference implementations
- `RATIONALE.md` — Why we made these decisions
- `Shared/WORKFLOW-QUICK.md` — Test generation workflow
- `Shared/DATA-RULES.md` — Test data self-sufficiency rules
