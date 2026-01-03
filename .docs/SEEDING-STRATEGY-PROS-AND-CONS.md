# Seeding Strategy: Pros & Cons

## Our Approach

**Pre-seed database with known test data. Fresh database for each full suite run.**

Tests use seeded data for read operations, create NEW data for CRUD testing (but don't reuse it).

---

## Why This Works

### Fast Parallel Execution
- **Multiple agents can run simultaneously** - no test interference
- Test any area in minutes, not hours
- No waiting for prerequisite tests to create data
- Example: 5 DevOps agents testing Login, Teams, Reports, Planning, ManageData in parallel

### Test Independence
- Run ANY test standalone (no dependencies)
- Tests can execute in any order
- Failures isolated (one bad test doesn't cascade)
- Jump straight to testing specific features

### Reliability
- **Same data every run** - deterministic, reproducible
- No state pollution between tests
- No cleanup complexity or failures
- Fresh start eliminates "it worked yesterday" mysteries

### CI/CD Friendly
- Parallel execution = faster pipelines
- No flaky "sometimes passes" due to data state
- Easy to identify real failures vs data issues

---

## Trade-offs (And Why They're Acceptable)

### "Seeding Scripts Grow Large"
**Reality:** Yes, ~400 lines for 9 users now, could reach 1000+ lines

**Mitigation:**
- USER INDEX makes it scannable
- One-time cost per test
- Alternative (create data in tests) has same maintenance + slower execution

**Verdict:** Acceptable - organization and documentation solve this

### "Fresh Database Takes 1-2 Minutes"
**Reality:** Yes, OneShot tests require DB recreation

**Mitigation:**
- Quick iterations: `dotnet test --filter "Category!=OneShot"` (no recreation)
- Full validation only before commits
- 2 min one-time cost vs unreliable state

**Verdict:** Acceptable - optimize for iteration speed (repeatable tests)

### "Data Coupling to IDs"
**Reality:** Yes, tests hardcode "User 9100" assumptions

**Mitigation:**
- USER INDEX documents all assumptions
- Constants for magic numbers
- Schema changes affect ANY approach

**Verdict:** Acceptable - explicit coupling is better than hidden coupling

### "Can't Test Some Dynamic Scenarios"
**Reality:** Volume testing, race conditions, real-world scale not covered

**Counter:**
- Those belong in performance/integration tests anyway
- UI tests verify UI behavior with representative data
- Edge cases CAN be seeded (duplicate usernames, locked accounts, etc.)

**Verdict:** Acceptable - different test types have different purposes

---

## What We Test vs Don't Test

### We DO Test (Important!)
- ✅ CRUD operations (Create Team, Edit User, Delete Task)
- ✅ UI workflows (Login → Navigate → Edit → Save)
- ✅ Validation (password requirements, required fields)
- ✅ Edge cases (duplicate usernames, inactive accounts, no roles)

### We DON'T Test
- ❌ Data persistence across sessions (API test concern)
- ❌ Database triggers and constraints (integration test concern)
- ❌ Performance at scale (performance test concern)
- ❌ Background jobs and schedulers (not UI testable)

### Critical Distinction
**We test CRUD functionality but don't DEPEND on it:**
- "Create Team" test creates a team to verify creation works ✅
- "View Team" test uses pre-seeded team (not the one from "Create Team") ✅
- This allows testing View without Create working first ✅

---

## Alternative Approaches (Why We Didn't Choose Them)

### Create Data In Tests
```csharp
[Fact]
public async Task CanViewTeam()
{
    // Create team first
    await CreateTeamAsync("Test Team");
    // Now view it
    await NavigateToTeamsAsync();
    // Assert...
}
```

**Problems:**
- Slow (DB operations in every test)
- Tests depend on creation working
- Cleanup required (or state pollution)
- Can't run tests in parallel
- Can't test specific features without running setup

### Shared State Between Tests
Use data created by previous tests.

**Problems:**
- Order dependency (tests must run in sequence)
- Cascading failures (one break = all break)
- No parallel execution
- Debugging nightmares

### Database Snapshots/Restore
Create snapshot, run tests, restore snapshot.

**Problems:**
- Complex infrastructure
- Slower than fresh seed (snapshot restore overhead)
- Still need seeding (to create snapshot)
- Doesn't solve the core issues

---

## Success Metrics

**Our approach delivers:**
- ✅ 15 tests in ~30 seconds (full suite)
- ✅ 7 tests in ~15 seconds (repeatable only)
- ✅ Tests run in any order
- ✅ Parallel execution by test class
- ✅ Zero cross-test pollution
- ✅ Instant debugging (same data every time)

**This enables:**
- Multiple developers working on different features simultaneously
- Fast feedback cycles (15s for quick iterations)
- Reliable CI/CD pipelines
- Easy test maintenance

---

## Bottom Line

**The game changer:** Test any area in minutes, not hours, with multiple agents running in parallel.

**Trade-offs exist** but are far outweighed by speed, reliability, and independence gains.

**This approach scales** with proper organization (USER INDEX, documentation, constants).
