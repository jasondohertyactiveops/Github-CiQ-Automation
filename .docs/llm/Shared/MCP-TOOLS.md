# MCP Tools Quick Reference

Commands for accessing source materials during test automation.

---

## Azure DevOps

### Get Specific Test Case
```
azureDevOps:get_work_item 
  workItemId=25057
```

### Search Test Cases
```
azureDevOps:search_work_items 
  projectId="Workware 7"
  searchText="login valid credentials"
```

### List Test Cases in Suite
```
azureDevOps:list_work_items 
  projectId="Workware 7"
  queryId=<suite_query_id>
```

**Project:** Workware 7  
**Organization:** activeopsdev  
**URL:** https://dev.azure.com/activeopsdev/Workware%207/_testPlans

---

## Playwright (App Exploration)

### Navigate to Page
```
playwright:browser_navigate 
  url="http://ww7client.localhost/login"
```

### Get Page Snapshot
```
playwright:browser_snapshot
```

Returns accessible tree with semantic locators.

### Take Screenshot (If Needed)
```
playwright:browser_take_screenshot
  filename="login-page.png"
```

**When to use:** Finding semantic locators, verifying current app state, when Cypress uses data-tags.

---

## File Paths (Reference)

### Test Case Documentation
```
.docs/test-cases/UI/ClientApp/{Suite-ID}/
.docs/test-cases/API/ClientAPI/{Suite-ID}/
```

### Test Code
```
src/AO.Automation.UI.Client/Tests/{Feature}/
src/AO.Automation.API.Client/Tests/{Feature}/
```

### Seeding Scripts
```
WW7/ww7-api/AO.WW/AO.WW.DB.Client/Scripts/InitialClientSeeding/Automation/
```

### Cypress (Legacy Reference)
```
WW7/Projects/ControliQAutomation/cypress/e2e/
```

---

## Database Access

### Check Schema
```
Filesystem:read_text_file
  path="D:\ActiveOpsGit\WW7\ww7-api\AO.WW\AO.WW.DB.Client\Tables\User.sql"
```

### View Seeding Script
```
Filesystem:read_text_file
  path="D:\ActiveOpsGit\WW7\ww7-api\AO.WW\AO.WW.DB.Client\Scripts\InitialClientSeeding\Automation\UiTestUsers.sql"
```

---

## Workflow Integration

1. **Fetch test case:** Azure DevOps MCP
2. **Explore app:** Playwright MCP (if needed)
3. **Check Cypress:** Filesystem tools (reference only)
4. **Create test case MD:** Filesystem write
5. **Generate test code:** Filesystem write
6. **Update seeding:** Filesystem edit (WW7 repo)

See WORKFLOW-QUICK.md for complete process.
