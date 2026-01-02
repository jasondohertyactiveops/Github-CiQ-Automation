# Seeding Strategy

## Why We Seed

**Advantages:**
- **Fast** - No database operations during tests
- **Isolated** - Each test uses dedicated user/data
- **Repeatable** - Same data every run
- **No cleanup** - Fresh database for each full suite run
- **No test dependencies** - Tests don't rely on other tests having run first

**Independence principle:**
- We DO test data creation/updates (CRUD operations)
- We DON'T use the results of those tests for other tests
- Example: "Create Team" test creates a team to verify creation works, but "View Team" test uses pre-seeded team
- This allows testing ANY area without running tests in specific order

**Trade-off:**
- Doesn't use test-created data (intentional - for independence)
- Seeding scripts grow with test suite
- Requires database recreation for OneShot tests

## User ID Ranges

**Reserved: 9000-9999** (StaffMember and User tables)

**Current allocations:**
```
9001-9002: TC24155 - Username Reuse After Deletion
9003:      TC24166 - Account Activation (OneShot)
9004-9005: TC25058 - Invalid Credentials (OneShot)
9006:      TC25061 - Reset Password from Login (OneShot)
9007:      TC29202 - Change Password from My Account (OneShot)
9008:      TC29201 - Forced Password Change (OneShot)
9100:      automation.teammember1 - General reusable user
9101:      automation.teammember2 - Language testing (OneShot)
```

**See:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Scripts/InitialClientSeeding/Automation/AutomationTestUsers.sql`

## Adding New Test Users

**1. Choose next available ID** (9009, 9010, etc.)

**2. Add to AutomationTestUsers.sql:**
```sql
-- TC#####: Your Test Name
SET IDENTITY_INSERT [StaffMember] ON;
INSERT INTO [StaffMember] (...) VALUES (90XX, ...);
SET IDENTITY_INSERT [StaffMember] OFF;

-- Link to workgroup, user type, etc.

SET IDENTITY_INSERT [User] ON;
INSERT INTO [User] (...) VALUES (90XX, ...);
SET IDENTITY_INSERT [User] OFF;

-- Assign role
INSERT INTO [UserToWorkgroupsToRoles] VALUES (1, 90XX, 44, 1, 0);
```

**3. Update USER INDEX comment** at top of file

**4. Recreate database:**
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

## Standard User Template

**Password:** `Workware@1` (hash: `Czhkx4C4AEjHzS0uzrQ49w==|Tf6mJxm7vOdRHojae+nvDGyAm8FBffKZdBQ5ZW5zwLY=`)  
**Workgroup:** 44 (Team 1)  
**Role:** 1 (All Access)  
**UserType:** 1 (Team Member)  
**Active:** 1  
**ActivationStatusId:** 4 (Activated)

**Modify as needed** for specific test scenarios (inactive, no role, etc.)

## Reusable vs OneShot Users

**Reusable Users (9100+):**
- Use for tests that don't modify user state
- Can be shared across multiple tests
- Examples: automation.teammember1

**OneShot Users (9000+):**
- Use for tests that permanently modify user
- Consumed after one test run
- Examples: activation, password changes, account lockouts

## Database Recreation

**When required:**
- Before running full test suite
- After running OneShot tests
- When seeding scripts change
- When test data becomes corrupted

**How:**
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

**Duration:** ~1-2 minutes
