# CLAUDE.md

Convert manual Azure DevOps test cases into automated Playwright .NET tests through collaborative, step-by-step workflow.

## Read First

Before doing anything, read these docs:
1. `.docs/llm/Shared/LLM-GUIDE.md` - Environment, capabilities, constraints
2. `.docs/llm/Shared/WORKFLOW-QUICK.md` - Test generation workflow
3. `.docs/llm/Shared/DATA-RULES.md` - Test data self-sufficiency rules

Domain-specific docs:
- UI: `.docs/llm/UI/` (patterns, project instructions, test case instructions)
- API: `.docs/llm/API/` (patterns, project instructions, test case instructions)

## How We Work

This is NOT "automate everything automatically". This is:
- One test suite or case at a time
- Show what you found, discuss, create docs, generate code, user runs, you fix
- Ask questions, push back when not sure
- Check in at each step

## Key Principles

- Tests must be self-sufficient (no test-to-test dependencies)
- Pre-seeded data: users 9000-9199 UI, 9200-9299 API
- OneShot tests OK (mark on methods with `[Trait("Category", "OneShot")]`)
- Traceability via attributes: `[AzureTestSuite]` → `[AzureTestCase]` → `[AzureTestStep]` → `[AzureTestPlan]`

## Traceability Attributes

Class level (order matters):
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
[Trait("Category", "OneShot")]  // if applicable, method level only
```

Convention tests enforce these — build will fail if missing.

## Project Structure

```
src/
  AO.Automation.Shared/        # Shared attributes, config, helpers
  AO.Automation.API.Client/    # API test project (xUnit + Dapper)
  AO.Automation.UI.Client/     # UI test project (Playwright .NET + xUnit)
.docs/
  test-cases/UI/ClientApp/     # UI test case documentation
  test-cases/API/ClientAPI/    # API test case documentation
  llm/                         # LLM guidance docs
```

## Communication Style

- Keep answers brief
- Don't summarise changes visible in git
- Commit messages: just the message, not the command, keep them concise
- Don't swear, even if the user does
