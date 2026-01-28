# Test Generation Workflow

From Azure DevOps test case → working test code with traceability.

**IMPORTANT:** This is a collaborative, iterative process. Each step involves discussion and confirmation.

---

## The Collaboration Pattern

**You say:** "Let's automate test case 25057"
**I do:** Fetch from Azure DevOps, show you what I found
**We discuss:** Is this Pure-UI? What data needed? 
**I create:** Test case MD
**You review:** "Looks good" or "Change X"
**I generate:** Test code
**You run:** Test and show me results
**I fix:** Based on your feedback
**Repeat:** Until test passes

This is **not** "fetch 10 test cases and auto-generate everything". It's **one test at a time, checking in at each step**.

---

## Step 1: Fetch Test Case from Azure DevOps

### Get Test Suite
```
azureDevOps:list_work_items 
  projectId="Workware 7"
  queryId=<suite_query_id>
```

### Get Specific Test Case
```
azureDevOps:get_work_item workItemId=25057
```

**Capture:**
- Test case ID (e.g., 25057)
- Title
- Suite ID (e.g., Login-25146)
- Order in suite (1, 2, 7, 12)
- Steps and expected results

---

## Step 2: Explore Current Application (If Needed)

### When to use Playwright MCP
- Cypress uses data-tags (need semantic locators)
- Azure test case vague (need to see actual UI)
- Unclear what elements exist

### Commands
```
playwright:browser_navigate url="http://ww7client.localhost/login"
playwright:browser_snapshot
```

**Capture semantic locators:**
- `Page.GetByRole(AriaRole.Button, new() { Name = "Login" })`
- `Page.GetByLabel("Username")`
- `Page.GetByText("Welcome")`

**Document in test case MD** under "Automation Approach"

---

## Step 3: Determine Category

**Pure-UI-Appropriate?**
- ✅ Tests user-visible behavior
- ✅ User can do it in browser
- ✅ Not time-based, not backend auth, not email system

**If NO:**
- Add `-NOT-UI` suffix
- Document reason (Time-Based, Backend Auth, Email System, NFR, Backend Business Rule)

**If Unclear:**
- Add `-TODO` suffix
- Document questions

---

## Step 4: Create Test Case MD

### Location & Naming
```
.docs/test-cases/{Type}/{App}/{Suite-ID}/{Order}-TC{ID}-{Name}[-Suffix].md
```

**Examples:**
- `.docs/test-cases/UI/ClientApp/Login-25146/07-TC25057-ValidCredentialsLogin.md`
- `.docs/test-cases/UI/ClientApp/Login-25146/01-TC25059-EmailLinkExpiry-NOT-UI.md`

### Required Sections
1. **Source Analysis** - Azure + Cypress + Current app
2. **Final Test Specification** - What to test, prerequisites, steps, expected results
3. **Automation Approach** - Pattern (A/C), Page Objects, Locators
4. **Notes** - Important context

See `UI/TESTCASE_INSTRUCTIONS.md` for template.

---

## Step 5: Identify Data Requirements

### OneShot or Repeatable?
**OneShot** - Modifies data permanently (activate, change password, lock account)
**Repeatable** - Just reads data (login, view page)

### Allocate User ID
- UI: 9000-9199
- API: 9200-9299

### Update Seeding Scripts (If Needed)
Location: `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Scripts/InitialClientSeeding/Automation/`

Then recreate database:
```powershell
cd D:\ActiveOpsGit\WW7\misc\Docker\local-environment
.\recreate-databases.ps1
```

### Data Self-Sufficiency Rule
**CRITICAL:** Each test must be self-sufficient. No test creates data for another test.

See `DATA-RULES.md` for complete rules.

---

## Step 6: Generate Test Code

### Location & Naming
```
src/AO.Automation.UI.Client/Tests/{Feature}/{Name}.cs
```

Match test case MD name (without order prefix and suffix):
- MD: `07-TC25057-ValidCredentialsLogin.md`
- Code: `ValidCredentialsLogin.cs`

### Template
```csharp
/// <summary>
/// TC{ID}: {Title}
/// </summary>
[Trait("Suite", "{Suite}-{ID}")]
[Trait("Feature", "{Feature}")]
[Trait("Category", "OneShot")]  // If applicable
public class {Name} : PlaywrightTest, IClassFixture<BrowserFixture>
{
    [Fact]
    public async Task {MethodName}()
    {
        // Persona user: {username} (User {ID})
        // AD: Step 1 - {Description}
        
        // Implementation with step comments
    }
}
```

### Traceability Elements
- Class comment: `/// TC25057:`
- Trait: `[Trait("Suite", "Login-25146")]`
- User comment: `// Persona user: ... (User 9100)`
- Step comments: `// AD: Step 1 -`, `// AD: Step 2 -`

---

## Step 7: Run & Verify

```powershell
cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation.UI.Client
dotnet test --filter "FullyQualifiedName~ValidCredentialsLogin"
```

**For OneShot tests:** Recreate database after first run.

---

## Step 8: Commit

### Two-Repo Strategy

**If seeding updated (WW7):**
```
seed: add user 9XXX for TC{ID}
```

**Then automation (Github-CiQ-Automation):**
```
test: automate TC{ID} {title}

- Add test case documentation
- Implement {ClassName}
- Uses seeded user 9XXX
```

---

## Quick Reference

### File Paths
- Azure DevOps: https://dev.azure.com/activeopsdev/Workware%207/_testPlans
- Test Case MDs: `.docs/test-cases/{Type}/{App}/{Suite-ID}/`
- Test Code: `src/AO.Automation.UI.Client/Tests/{Feature}/`
- Seeding: `WW7/ww7-api/.../Automation/UiTestUsers.sql`
- User Reference: `.docs/llm/Shared/SEEDING-REFERENCE.md`

### MCP Tools
- Azure DevOps: `azureDevOps:get_work_item`, `azureDevOps:list_work_items`
- Playwright: `playwright:browser_navigate`, `playwright:browser_snapshot`

### Naming Pattern
```
Azure TC25057 in suite Login-25146 at position 7
  ↓
07-TC25057-ValidCredentialsLogin.md
  ↓
ValidCredentialsLogin.cs
```

### Search/Trace
- Find by TC ID: Search for `TC25057`
- Find by Azure step: Search for `// AD: Step 2`
- Find by suite: Search for `[Trait("Suite", "Login-25146")]`
