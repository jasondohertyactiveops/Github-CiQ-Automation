# Testing Guide

## Test Execution

### Full Suite (All Tests)
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1

cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation
dotnet test
```
**Duration:** ~30 seconds  
**When:** Before commits, CI/CD pipeline

### Quick Iterations (Repeatable Only)
```powershell
dotnet test --filter "Category!=OneShot"
```
**Duration:** ~15 seconds  
**When:** During development, debugging  
**Skips:** 8 OneShot tests that require fresh database

### Single Test
```powershell
dotnet test --filter "FullyQualifiedName~TestClassName"
```

### By Feature
```powershell
dotnet test --filter "Feature=Login"
dotnet test --filter "Feature=MyAccount"
```

## Test Categories

### OneShot Tests
**What:** Tests that modify data permanently (activate users, change passwords, lock accounts)  
**Requirement:** Fresh database for each run  
**Examples:** TC24166 (activation), TC25058 (invalid login locks accounts), TC29202 (password change)

**Identifying OneShot:**
```csharp
[Trait("Category", "OneShot")]
```

### Repeatable Tests
**What:** Tests that only read data or restore state at end  
**Requirement:** Can run multiple times on same database  
**Examples:** TC24155 (username reuse), TC25057 (valid login), TC24230 (view account)

## Test Isolation

**Parallel Execution:**
- Different test classes run in parallel (xUnit default)
- Tests within same class run sequentially
- Each class shares one BrowserFixture (one browser per class)

**Data Isolation:**
- Each test uses dedicated user (see SEEDING-STRATEGY.md)
- No shared state between tests
- OneShot tests consume their user permanently

## Debugging

**Run with Browser Visible:**
```json
// appsettings.json
"Headless": false
```

**Single Test with Breakpoints:**
- Set breakpoint in test
- F5 in Visual Studio or Rider
- Or: `dotnet test --filter "FullyQualifiedName~TestName"` with debugger attached

**Check Test Logs:**
- Console output shows which tests ran
- Playwright trace/screenshots in bin/Debug/net10.0/ (if configured)

## Common Issues

See `TROUBLESHOOTING.md` for detailed solutions.
