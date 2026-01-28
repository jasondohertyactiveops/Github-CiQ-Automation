# Test Data Rules

**Core Principle:** Tests must be self-sufficient. No test creates data for another test to use.

---

## Rule 1: No Test-to-Test Dependencies

### ❌ BAD
```csharp
[Fact, Priority(1)]
public async Task CreateTeam() { _sharedTeamId = "..."; }

[Fact, Priority(2)]
public async Task EditTeam() { /* uses _sharedTeamId */ }
```

**Problem:** Test 2 depends on Test 1. Can't run in isolation, can't run in parallel, failures cascade.

### ✅ GOOD
```csharp
[Fact]
public async Task CanCreateTeam() { /* creates, verifies, done */ }

[Fact]
public async Task CanEditTeam() { /* uses PRE-SEEDED team OR creates own */ }
```

**Why:** Independent, can run in any order, no cascading failures.

---

## Rule 2: Use Pre-Seeded Data

**Strategy:** Database starts with known test users/data.

```csharp
[Fact]
public async Task CanViewTeam()
{
    // Use pre-seeded team (documented in SEEDING-REFERENCE.md)
    var teamId = "550e8400-e29b-41d4-a716-446655440000";
    await teamsPage.NavigateToTeam(teamId);
}
```

**Benefits:** Fast, reliable, documented, no setup needed.

---

## Rule 3: OneShot Tests Are OK

**OneShot:** Permanently modifies data, can't re-run without database reset.

**Examples:** Activate account, change password, lock account

**Mark clearly:**
```csharp
[Trait("Category", "OneShot")]
public class AccountActivation : PlaywrightTest
{
    // Test activates user 9003 - can only run once per DB
}
```

**Run strategy:**
```powershell
# Skip OneShot during development
dotnet test --filter "Category!=OneShot"

# Full run (requires DB recreate after)
dotnet test
.\recreate-databases.ps1
```

---

## Rule 4: Each Test Area is Self-Sufficient

**Feature areas own their data:**
- Login: Users 9000-9020
- Teams: Users 9100-9120
- Capacity: Users 9120-9150
- API: Users 9200-9299

**No cross-suite dependencies.**

---

## Rule 5: If Test Creates, Test Owns

### Strategy 1: Create with Unique Name
```csharp
var teamName = $"Test Team {Guid.NewGuid()}";
await teamsPage.CreateTeam(teamName);
// Leave in DB (no conflict due to unique name)
```

### Strategy 2: Create, Use, Delete
```csharp
var teamName = $"Delete Test {Guid.NewGuid()}";
await teamsPage.CreateTeam(teamName);
await teamsPage.DeleteTeam(teamName);
// Clean - deleted own data
```

### Strategy 3: Use Pre-Seeded
```csharp
var preSeededTeamId = "550e8400-...";
await teamsPage.NavigateToTeam(preSeededTeamId);
// Read only - didn't modify
```

---

## Rule 6: Document Requirements

**In test case MD:**
```markdown
### Prerequisites
**Seeded user:**
- User ID: 9003
- Username: tc24166.activation@activeops.com
- Status: Invited
```

**In test code:**
```csharp
// Persona user: tc24166.activation (User 9003)
// OneShot: User 9003 consumed by this test
```

**In SEEDING-REFERENCE.md:**
Keep central registry of all test users.

---

## Decision Framework

**Question 1: Does test modify data permanently?**
- YES → OneShot, allocate dedicated user
- NO → Repeatable, use shared user

**Question 2: Does test need specific data?**
- YES → Pre-seed exactly what's needed
- NO → Use generic user (9100, 9200)

**Question 3: Does test create entities?**
- Unique names → Create and leave
- Fixed names → Create, use, delete
- Just testing creation → Create, verify, done

---

## Common Scenarios

| Scenario | Strategy |
|----------|----------|
| Read-only test | Use pre-seeded, don't modify |
| OneShot test | Dedicated user, mark clearly |
| Create entity | Unique name or create-use-delete |
| Update entity | Use pre-seeded OR create own |
| Delete entity | Create first, then delete |
| Complex workflow | All steps in one test |
| Independent checks | Separate tests, pre-seeded data |

---

## Anti-Patterns

❌ Static shared state between tests  
❌ Setup/teardown creates test data  
❌ Test order dependencies (Priority attributes)  
❌ One test creates data, another uses it

---

## Quick Reference

- **Self-sufficient:** Each test independent, can run alone
- **Pre-seeded:** Use known data from database
- **OneShot OK:** Mark with trait, document user consumption
- **Feature ownership:** Each area has its own user pool
- **Create unique:** If test creates, use unique names
- **Document:** Test case MD, code comments, SEEDING-REFERENCE.md

See SEEDING-REFERENCE.md for complete user registry.
