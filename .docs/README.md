# ControliQ Pure UI Automation

Playwright .NET test automation for ControliQ web application.

## Quick Start

**Prerequisites:**
- Docker Desktop running
- SQL Server container (db-server) running
- API and frontend containers running

**Run Tests:**
```powershell
# Fresh database (required for OneShot tests)
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1

# Run all tests
cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation
dotnet test

# Quick iterations (skip OneShot tests)
dotnet test --filter "Category!=OneShot"
```

## Project Structure

```
src/AO.Automation/
├── Tests/                      # Test classes organized by feature
│   ├── Login/                  # Login suite tests
│   ├── Admin/Account/MyAccount/# My Account tests
│   └── Utilities/              # Token generators, utilities
├── Pages/                      # Page Object Models
├── Helpers/                    # TokenHelper, utilities
└── BaseClasses/               # Base test classes, fixtures
```

## Test Categories

**Repeatable (7 tests):** Can run multiple times without fresh database
- TC24155, TC25057, TC24230 (3 methods)

**OneShot (8 tests):** Require fresh database to re-run
- TC24166, TC25058 (3 methods), TC25061, TC29202, TC25688, TC29201

## Current Coverage

**Login Suite (Login-25146): 11/12 tests (92%)**
- See `.docs/test-cases/Login-25146/` for detailed test specifications

## Key Features

- **Runtime token generation** - No expiry issues
- **Seeded test data** - Fast, isolated, repeatable
- **Semantic locators** - No data-tag pollution in production
- **Parallel execution** - Tests run in parallel by class
- **Fast execution** - Full suite ~30 seconds

## More Information

- **Testing Guide:** `.docs/TESTING-GUIDE.md`
- **Seeding Strategy:** `.docs/SEEDING-STRATEGY.md`
- **Troubleshooting:** `.docs/TROUBLESHOOTING.md`
- **LLM Instructions:** `.docs/llm/`
