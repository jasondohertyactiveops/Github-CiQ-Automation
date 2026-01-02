# Troubleshooting Guide

## Common Issues

### Tests Fail: "Token is not yet valid"

**Symptom:** API returns 401, logs show token ValidFrom is in future

**Cause:** FakeTime container clock is out of sync

**Solution:**
```powershell
# FakeTime uses file modification timestamp as base time
# Start scripts now auto-fix this, but if you see it:
(Get-Item "C:\DockerMount\FakeTime\timedefault.rc").LastWriteTime = Get-Date

# Restart containers
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\stop-app-containers.ps1
.\start-app-containers.ps1
```

### Tests Fail: "Link Invalid" on Activation/Reset

**Symptom:** Activation/reset page shows "Link Invalid" message

**Cause:** SecurityStamp mismatch between token and database

**Solution:**
- Ensure database was recreated after changing AutomationTestUsers.sql
- Verify token generation uses correct SecurityStamp from seeding script
- Check User table: `SELECT SecurityStamp FROM [User] WHERE Id = 90XX`

### OneShot Tests Fail on Re-run

**Symptom:** Activation fails with "user already activated", login fails with "locked out"

**Cause:** OneShot tests modify data permanently

**Solution:**
```powershell
# Recreate database to reset all test users
.\recreate-databases.ps1
```

**Prevention:** Use `dotnet test --filter "Category!=OneShot"` for quick iterations

### RTM Dialog Blocks Clicks

**Symptom:** Test times out trying to click user menu, log shows "dialog intercepts pointer events"

**Cause:** "Select Your Activity" dialog appears after RTM login

**Solution:**
- LoginPage.LoginAsync() now auto-closes this dialog
- If you see this error, LoginPage might need adjustment
- Verify dialog close button: `GetByTestId("close-btn")`

### Nginx 404 on Direct Routes

**Symptom:** Navigating to `/activateaccount/{token}` returns 404

**Cause:** Nginx default config doesn't handle SPA client-side routing

**Solution:**
- Already fixed in `ww7-client-app/nginx.conf`
- If you see this, rebuild frontend: `docker compose --profile frontend --profile automation build --no-cache`

### Permission 117 (Languages) Not Available

**Symptom:** General Preferences tab doesn't appear on My Account page

**Cause:** Role 1 missing permission 117

**Solution:**
- Already fixed in SeedInitialClientData.sql
- If you see this, check: `SELECT * FROM PermissionsToRole WHERE RoleId=1 AND PermissionId=117`
- Should return one row

### Browser Doesn't Close After Tests

**Symptom:** Chromium processes left running

**Cause:** Test crashed before browser disposal

**Solution:**
```powershell
# Kill all Chromium processes
Get-Process | Where-Object {$_.ProcessName -like "*chrome*"} | Stop-Process -Force
```

### Tests Run Sequentially (Slow)

**Symptom:** Tests take 2+ minutes for full suite

**Cause:** Probably running with wrong configuration

**Solution:**
- xUnit runs test classes in parallel by default
- Each class should have `IClassFixture<BrowserFixture>`
- Check test output - should see parallel execution

## Performance Expectations

**Full Suite (15 tests):** 30-40 seconds  
**Repeatable Only (7 tests):** 15-20 seconds  
**Single Test:** 2-8 seconds

**If slower:** Check for network issues, Docker resource limits, or orphaned browser processes
