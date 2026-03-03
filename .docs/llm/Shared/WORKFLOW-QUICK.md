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

## Understanding UI vs API Test Split

**CRITICAL:** Most Azure DevOps test cases require BOTH UI and API test coverage.

### The Analysis Process

**Step 1: Analyze Cypress**
- Check what's currently being tested in Cypress
- Understand the actual application behavior
- Identify what needs UI coverage vs API coverage

**Step 2: Split the Testing**

**UI Tests = Shallow (User-Facing Only):**
- Navigation and page display
- Form interactions and field validations
- Error messages and success toasts
- Grid display and filtering
- User workflows (navigate → fill form → submit → see result)
- **NO database checks**
- **NO backend verification**

**API Tests = Deep (Backend + Database):**
- HTTP operations (POST, PUT, DELETE, GET)
- Response structure and status codes
- **Database persistence verification**
- Business rule validation at API level
- Data integrity checks after operations

**Ignore (For Now):**
- Audit logging (separate initiative)
- Internationalization/French translations (separate initiative)

**Example: TC12345 "Create Entity"**

**UI Test Case (`UI/ClientApp/FeatureArea-12300/01-TC12345-CreateEntity.md`):**
- Navigate to entity management page
- Fill form with required fields
- See validation errors for missing/invalid fields
- Submit successfully
- See success message
- Verify item appears in grid
- Search/filter for created item

**API Test Case (`API/ClientAPI/FeatureArea-12300/01-TC12345-CreateEntity.md`):**
- POST to create entity
- Check 201 response with correct structure
- **Check database table has the record**
- **Verify all fields persisted correctly**
- PUT to update entity
- **Check DB reflects update**
- DELETE entity
- **Check DB confirms deletion**

### Folder Structure

Test cases are organized by type:
- `UI/ClientApp/{Suite-ID}/` - User interface tests
- `API/ClientAPI/{Suite-ID}/` - Backend API tests

**Naming:** Same filename in both folders (no -UI or -API suffix needed).

---

## Step 1: Get Test Case from Azure DevOps

**CRITICAL:** User will provide the specific test case ID to automate. DO NOT use `azureDevOps:search_work_items` to fetch suite contents - this returns hundreds of irrelevant results.

### When User Says: "Let's automate TC25074 from suite 25155"

**Step 1a - Get the specific test case:**
```
azureDevOps:get_work_item workItemId=25074 expand="all"
```

**Step 1b - Get suite info (for naming):**
```
azureDevOps:get_work_item workItemId=25155
```

**Capture from test case:**
- Test case ID (e.g., 25074)
- Title (e.g., "User should be able to create workgroups")
- Steps and expected results
- Tags (e.g., "Admin and System Settings")

**Capture from suite:**
- Suite title (e.g., "Admin/System Settings")
- Use for folder naming: `AdminSystemSettings-25155`

**DO NOT:**
- Search for all test cases in a suite (user provides specific IDs)
- Use `azureDevOps:search_work_items` for suite contents
- Fetch multiple test cases at once (one at a time only)

---

## Step 2: Check Cypress Legacy Implementation (If Exists)

**Location:** `WW7/Projects/ControliQAutomation/cypress/e2e/`

**What to look for:**
- Does a test exist for this TC?
- What does it actually test?
- What data-tags does it use? (we'll replace with semantic locators)
- What's missing or wrong?

**Use Filesystem tools:**
```
Filesystem:search_files 
  path="{WW7_REPO}/Projects/ControliQAutomation"
  pattern="*login*.cy.js"
```

**Cypress is reference only** - we're replacing it with Playwright.

---

## Step 3: Explore Current Application (If Needed)

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

## Step 4: Determine Category

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

## Step 5: Create Test Case MDs (UI + API)

**CRITICAL:** Most test cases require BOTH UI and API test case MDs.

### Location & Naming
```
UI:  .docs/test-cases/UI/ClientApp/{Suite-ID}/{Order}-TC{ID}-{Name}.md
API: .docs/test-cases/API/ClientAPI/{Suite-ID}/{Order}-TC{ID}-{Name}.md
```

**Examples:**
- `UI/ClientApp/AdminSystemSettings-25155/01-TC25074-CreateWorkgroups.md`
- `API/ClientAPI/AdminSystemSettings-25155/01-TC25074-CreateWorkgroups.md`
- `UI/ClientApp/Login-25146/01-TC25059-EmailLinkExpiry-NOT-UI.md` (Pure-UI inappropriate)

**DO NOT create README.md files in test suite folders** - all information should be in individual test case MDs.

### Creating Both Variants

**Workflow:**
1. Create UI test case MD first (user-visible behaviors)
2. Create API test case MD second (backend operations + DB checks)
3. Review both together to ensure complete coverage
4. No duplication - each tests different concerns

**UI Test Case Focus:**
- Navigation and page display
- Form interactions and validations
- Error messages and success toasts
- Grid filtering and search
- User workflows

**API Test Case Focus:**
- HTTP operations (POST, PUT, DELETE, GET)
- Database persistence verification
- Response structure validation
- Business rule enforcement
- Data integrity checks

### Required Sections (Both UI and API)
1. **Source Analysis** - Azure + Cypress + Current app
2. **Final Test Specification** - What to test, prerequisites, steps, expected results
3. **Data Requirements** - User needs, pre-seeded data with SQL scripts, test data strategy
4. **Automation Approach** - Pattern (A/C), Page Objects/API Clients, Locators/Endpoints
5. **Notes** - Important context

See `.docs/llm/UI/TESTCASE_INSTRUCTIONS.md` and `.docs/llm/API/TESTCASE_INSTRUCTIONS.md` for templates.

---

## Step 6: Identify Data Requirements

### Document in Test Case MD
**CRITICAL:** The Data Requirements section is instructions for implementation. It must include:

**For tests using pre-seeded data:**
```markdown
## Data Requirements

### Pre-Seeded Entity
**Entity:** [EntityName] (Type: [Type])

**Key Field Values:**
- **Id:** 9001 (INT, sequential - use IDENTITY_INSERT)
- **Name:** "Exact Name" (test searches for this)
- **Description:** `TC{TestCaseID}-{TestNumber}-{Rand1}-{Rand2}-{ItemNumber}` (traceable GUID pattern)
- **[OtherField]:** [Value with explanation if needed]
- **[OtherField]:** [Value]

**Purpose:** [Why this data is needed - what test scenario it enables]

**Test Behavior:**
- [What the test does with this data]
- [Any modifications made]
- **Category:** OneShot/Repeatable

**Implementation:** Use IDENTITY_INSERT for Id field when creating seeding script.
```

**GUID Pattern in Description Field:** `TC{TestCaseID}-{TestNumber}-{Rand1}-{Rand2}-{ItemNumber}`
- **Example:** `TC25074-04-f3d2-8a1b-00000001`
- **Parts:**
  - `TC25074` = Test case ID
  - `04` = Test number within suite (01-07)
  - `f3d2-8a1b` = Random hex (4 chars each section, generate once per test)
  - `00000001` = Item number (increment for multiple items: 00000001, 00000002, etc.)

**For Multiple Items (e.g., 3 workgroups for search test):**
```markdown
### Pre-Seeded Workgroups (3 items)

**Item 1: Alpha Department**
- **Id:** 9003
- **Description:** `TC25074-06-a7b3-c2d1-00000001`
- [other fields...]

**Item 2: Beta Group**  
- **Id:** 9004
- **Description:** `TC25074-06-a7b3-c2d1-00000002` (same random sections)
- [other fields...]

**Item 3: Zeta Department**
- **Id:** 9005
- **Description:** `TC25074-06-a7b3-c2d1-00000003` (same random sections)
- [other fields...]

**Traceability:**
- All 3 items share random sections `a7b3-c2d1` (shows grouping)
- Find all: `WHERE Description LIKE 'TC25074-06-%'`
```

**Why This Approach:**
- Test cases specify WHAT data is needed, not HOW to create it
- Avoids schema drift issues in documentation
- Implementation details (actual SQL) handled when creating seeding script
- Key field values documented for test logic (Name to search for, Description for traceability, etc.)
```

**For tests creating their own data:**
```markdown
## Data Requirements

### User
- **User 9100:** automation.teammember1@activeops.com
- **No pre-seeded entities needed**

### Test Data Strategy
- **Entity Name:** `$"AutoTest Entity {Guid.NewGuid()}"`
- **Why GUID:** Uniqueness, parallel execution, no cleanup
- **Self-Sufficient:** Creates own data
```

### OneShot or Repeatable?
**OneShot** - Modifies data permanently (activate, change password, lock account)
**Repeatable** - Just reads data (login, view page)

### Allocate User ID
- UI: 9000-9199
- API: 9200-9299

### Update Seeding Scripts (If Needed)
Location: `WW7/ww7-api/AO.WW/AO.WW.DB.Client/Scripts/InitialClientSeeding/Automation/`

**When to create seeding files:**
- After all test cases in a suite are documented
- Before implementing automation tests
- Consolidate related test data in one file (e.g., `UiTestWorkgroups.sql` for all workgroup tests)

**Workflow:**
1. Document SQL in test case MDs first (as instructions)
2. Implement all test cases in suite
3. Create consolidated seeding file from all SQL snippets
4. Recreate database

Then recreate database:
```powershell
# In the WW7 repo
cd misc/Docker/local-environment
.\recreate-databases.ps1
```

### Data Self-Sufficiency Rule
**CRITICAL:** Each test must be self-sufficient. No test creates data for another test.

See `DATA-RULES.md` for complete rules.

---

## Step 7: Generate Test Code

### Location & Naming
```
src/AO.Automation.UI.Client/Tests/{Feature}/{Name}.cs
```

Match test case MD name (without order prefix and suffix):
- MD: `01-TC25074-CreateWorkgroups.md`
- Code: `CreateWorkgroups.cs`

### Template
```csharp
[AzureTestSuite({SuiteID})] // {SuiteName}
[AzureTestCase({TestCaseID})]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class {Name} : PlaywrightTest, IClassFixture<BrowserFixture>
{
    [Fact]
    [AzureTestStep({TestCaseID}, 1)]
    [Trait("Category", "OneShot")]  // If applicable, method level only
    public async Task {MethodName}()
    {
        // Persona user: {username} (User {ID})
        // Implementation
    }
}
```

### Traceability Elements
- Suite: `[AzureTestSuite(25146)]` on class
- Case: `[AzureTestCase(25057)]` on class
- Plan: `[AzureTestPlan("Smoke")]` on class
- Step: `[AzureTestStep(25057, 1)]` on method
- OneShot: `[Trait("Category", "OneShot")]` on method only

---

## Step 8: Run & Verify

```powershell
cd src/AO.Automation.UI.Client
dotnet test --filter "FullyQualifiedName~CreateWorkgroups"
```

**For OneShot tests:** Recreate database after first run.

---

## Step 9: Commit

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
User provides: "TC25074 from suite 25155, order 1"
  ↓
01-TC25074-CreateWorkgroups.md
  ↓
CreateWorkgroups.cs
```

### Search/Trace
- Find by TC ID: Search for `AzureTestCase(25074)`
- Find by step: Search for `AzureTestStep(25074, 2)`
- Find by suite: Search for `AzureTestSuite(25155)`
- Filter pipeline: `dotnet test --filter "Plan=Smoke"` or `dotnet test --filter "Suite=25146"`
