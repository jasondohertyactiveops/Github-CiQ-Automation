# Seeding Reference for LLMs

Quick reference for test user IDs and test case mappings.

## Automation Test Users

**Location:** `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Scripts/InitialClientSeeding/Automation/AutomationTestUsers.sql`

**All users:**
- Password: `Workware@1` (unless noted)
- Workgroup: 44 (Team 1)
- Role: 1 (All Access)
- UserType: 1 (Team Member)

## User Index

| User ID | Username | Test Case | Type | Purpose |
|---------|----------|-----------|------|---------|
| 9001 | tc24155.duplicate@activeops.com | TC24155 | OneShot | Deleted user (username reuse) |
| 9002 | tc24155.duplicate@activeops.com | TC24155 | Repeatable | Active user (same username) |
| 9003 | tc24166.activation@activeops.com | TC24166 | OneShot | Account activation |
| 9004 | tc25058.norole@activeops.com | TC25058 | OneShot | User with no roles |
| 9005 | tc25058.inactive@activeops.com | TC25058 | OneShot | Inactive user |
| 9006 | tc25061.reset@activeops.com | TC25061 | OneShot | Password reset from login |
| 9007 | tc29202.passwordchange@activeops.com | TC29202 | OneShot | Change password from My Account |
| 9008 | tc29201.mustchange@activeops.com | TC29201 | OneShot | Forced password change (ChangePasswordOnLogin=1) |
| 9100 | automation.teammember1@activeops.com | TC25057, TC24230 | Repeatable | General team member |
| 9101 | automation.teammember2@activeops.com | TC25688 | OneShot | Language preference testing |

## Test Coverage Map

**Login Suite (Login-25146):**
- TC24155: Username Reuse After Deletion (Users 9001-9002) ✅
- TC24166: Account Activation (User 9003) ✅
- TC25057: Valid Credentials Login (User 9100) ✅
- TC25058: Invalid Credentials (Users 9004-9005) ✅
- TC25061: Reset Password from Login (User 9006) ✅
- TC24230: View My Account (User 9100) ✅
- TC25688: General Preferences (User 9101) ✅
- TC29202: Change Password from My Account (User 9007) ✅
- TC29201: Forced Password Change (User 9008) ✅
- TC24154: Users Without Email ❌ (TODO)
- TC25059: Email Link Expiry ❌ (NOT-UI)
- TC25060: Token Refresh ❌ (NOT-UI)

**Status: 11/12 tests automated (92%)**

## Special User Configurations

**User 9003 (Activation):**
- ActivationStatusId: 2 (Invited)
- SecurityStamp: 3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D (for token generation)
- PasswordHash: NULL (no password until activated)

**User 9004 (No Roles):**
- Has StaffMember, User records
- NO UserToWorkgroupsToRoles entry

**User 9005 (Inactive):**
- User.Active: 0
- StaffMember.Active: 0

**User 9006 (Reset Password):**
- SecurityStamp: 4B2C3D4E-5F6A-7B8C-9D0E-1F2A3B4C5D6E (for reset token)

**User 9008 (Forced Change):**
- ChangePasswordOnLogin: 1
- Password: Workware@1 (temporary)

## Token Generation

**TokenHelper location:** `src/AO.Automation/Helpers/TokenHelper.cs`

**Activation token:**
```csharp
var token = tokenHelper.GenerateActivationToken(
    clientIdentifier: "ww7client",
    staffMemberId: 9003,
    email: "tc24166.activation@activeops.com",
    securityStamp: "3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D"
);
```

**Reset password token:**
```csharp
var token = tokenHelper.GenerateResetPasswordToken(
    clientIdentifier: "ww7client",
    staffMemberId: 9006,
    username: "tc25061.reset@activeops.com",
    securityStamp: "4B2C3D4E-5F6A-7B8C-9D0E-1F2A3B4C5D6E"
);
```

## Quick Commands

**Recreate database:**
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

**Run all tests:**
```powershell
cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation
dotnet test
```

**Run repeatable only:**
```powershell
dotnet test --filter "Category!=OneShot"
```

**Check user in DB:**
```powershell
docker exec -it db-server /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'yourStrong(!)Password' -C -Q "SELECT Id, UserName, ActivationStatusId, ChangePasswordOnLogin FROM WW7Client.dbo.[User] WHERE Id = 90XX"
```
