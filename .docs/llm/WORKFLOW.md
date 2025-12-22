# Workflow Guide

## How to Automate a Test Case

### The Basic Cycle

```
1. You identify test case → 2. I fetch from Azure → 3. I generate code → 4. You run it → 5. I fix issues → 6. You commit
```

### Step-by-Step Process

#### Step 1: Identify Test Case to Automate
**You provide:**
- Azure Test Case ID (e.g., "Automate TC2813")
- OR Test Suite name (e.g., "Automate all tests in Login Suite")
- OR Feature area (e.g., "I need tests for capacity planning")

#### Step 2: I Fetch Test Case Details
**I will:**
- Use Azure DevOps MCP to fetch test case
- Show you the test case title and steps
- Confirm it's the right one

**Example:**
```
You: "Automate test case 2813"
Me: [Fetches TC2813]
Me: "Got it - TC2813: 'Verify that the Login screen shows as expected'"
Me: Shows you the steps
Me: "Ready to generate the test?"
```

#### Step 3: I Generate Test Code
**I will create:**
- Test class file (e.g., `LoginScreenValidation.cs`)
- Page Object classes if needed (e.g., `LoginPage.cs`)
- Helper methods if needed
- Put files in appropriate folders

**I will NOT:**
- Run the code
- Commit to git
- Install packages

**Example output:**
```
Created:
- /Tests/Login/LoginScreenValidation.cs
- /Pages/LoginPage.cs

Ready for you to run: dotnet test --filter "Feature=Login"
```

#### Step 4: You Run the Test
**You do:**
```bash
cd D:\ActiveOpsGit\Github-CiQ-Automation\src\AO.Automation
dotnet test --filter "Feature=Login"
```

**Then tell me:**
- ✅ "Test passed!" → Done, ready for commit
- ❌ "Test failed with error: [paste error]" → I'll fix it
- ⚠️ "Can't find element X" → I'll adjust locators

#### Step 5: I Fix Issues
**If test fails, you provide:**
- Error message
- Screenshots if helpful
- What you observed

**I will:**
- Analyze the failure
- Update the code
- Explain what I changed

**We iterate until it passes.**

#### Step 6: You Commit
**When test is working:**
```bash
git add .
git commit -m "test: automate TC2813 login screen validation"
git push
```

**In Azure DevOps:**
- Create PR
- Link to work item #2813
- Merge when approved

---

## Common Workflows

### Automate Multiple Test Cases
```
You: "Automate TC2813, TC2815, TC2820"
Me: [Fetches all three]
Me: [Generates all three test classes]
You: [Run all: dotnet test --filter "Feature=Login"]
```

### Automate Full Test Suite
```
You: "Get all test cases from Login Suite"
Me: [Fetches test suite, lists all test cases]
Me: "Found 15 test cases. Should I generate all of them?"
You: "Yes" or "Just the first 5" or "Skip TC2820"
```

### Fix Flaky Test
```
You: "Test LoginScreenValidation is flaky, fails randomly"
Me: "What's the failure message?"
You: [Provides error]
Me: [Analyzes, suggests fix - likely wait/timing issue]
Me: [Updates code with better waits]
You: [Runs multiple times to verify]
```

### Reproduce Production Bug
```
You: "Bug #12345 - User can't save team with special characters"
Me: "Should I write a failing test that reproduces this?"
You: "Yes, team name should be 'Test & Dev'"
Me: [Creates test that demonstrates the bug]
You: [Runs test, confirms it fails as expected]
You: [Fixes bug in app code]
You: [Runs test again, now passes]
```

### Add Test for New Feature
```
You: "New feature: bulk assign staff. Here's the flow: [describes steps]"
Me: "Should I create a new test class or add to existing?"
You: "New class: BulkStaffAssignment.cs"
Me: [Creates test class with the workflow]
You: [Runs against feature branch]
```

---

## Working with Page Objects

### When I Create a New Page
```
Me: "Created LoginPage.cs with these methods:
- LoginAsync(username, password)
- ClickResetPassword()
- GetErrorMessage()"

You can now use this in other tests.
```

### When to Request a Page Object
```
You: "I'm seeing repeated code for navigating to capacity planning"
Me: "Should I create a CapacityPlanningPage?"
You: "Yes please"
Me: [Creates CapacityPlanningPage with navigation and common actions]
```

---

## Using Azure DevOps MCP

### Fetch Single Test Case
```
You: "Show me test case 2813"
Me: [Calls azureDevOps:get_work_item with workItemId=2813]
Me: [Displays title, steps, description]
```

### Search for Test Cases
```
You: "Find all test cases tagged with 'Login'"
Me: [Calls azureDevOps:search_work_items with searchText="Login"]
Me: [Lists results]
```

### Get Test Suite
```
You: "What's in the Smoke Test Suite?"
Me: [Searches for suite, gets test cases]
Me: [Lists all test cases in suite]
```

---

## Commit Message Conventions

**When you commit, use these formats:**

### Automating a test case:
```
test: automate TC2813 login screen validation

- Add LoginScreenValidation test class
- Create LoginPage page object
- Covers all 4 validation steps
```

### Fixing a flaky test:
```
fix: improve wait conditions in LoginScreenValidation

- Replace fixed waits with Playwright auto-waits
- Add explicit wait for SSO button when present
- Fixes random failures in CI
```

### Adding page object:
```
refactor: extract CapacityPlanningPage page object

- Create CapacityPlanningPage with common actions
- Update existing tests to use new page object
- Reduces code duplication
```

### Reproducing a bug:
```
test: add failing test for bug #12345

- Test demonstrates team name special character issue
- Currently fails as expected
- Will pass once bug is fixed
```

---

## Tips for Success

### 1. Start Small
- Automate 1 test, get it working, commit
- Don't try to automate 10 tests at once
- Build confidence with small wins

### 2. Run Tests Frequently
- After every change I make
- Catch issues early
- Don't let failures pile up

### 3. Provide Good Error Info
**Bad:** "Test failed"
**Good:** "Test failed with: 'Timeout waiting for selector .login-button'. Screenshot attached."

### 4. Keep Me Updated on Changes
- If app UI changes, tell me
- If locators break, show me the new HTML
- If workflows change, explain the new flow

### 5. Ask Questions
- Unsure about structure? Ask
- Want to change an approach? Discuss
- See a better pattern? Suggest it

---

## What I Can Help With

✅ Fetch test cases from Azure DevOps
✅ Generate test code (classes, page objects, helpers)
✅ Fix failing tests based on error messages
✅ Refactor code to improve structure
✅ Suggest better locators or patterns
✅ Explain what code does
✅ Suggest commit messages

---

## What I Cannot Do

❌ Run tests (you run them)
❌ Install packages (you install via dotnet/npm)
❌ Commit to git (you commit)
❌ Access the running application (you describe what you see)
❌ Debug without error messages (you provide logs/errors)

---

## Questions During Development

### "Should this be Pattern A or Pattern C?"
- Pure validation (checking elements exist)? → Pattern C
- Workflow with dependent steps? → Pattern A
- Ask me if unsure, I'll recommend

### "Should this be a new Page Object?"
- Used in 3+ tests? → Yes
- Complex interactions? → Yes
- Simple form? → Maybe wait until needed

### "This locator isn't working"
- Show me the error
- If possible, show me the HTML
- I'll suggest alternatives (GetByRole, CSS, etc.)

### "Test is too slow"
- Check for unnecessary waits
- Can we use pre-seeded data instead of creating?
- Can we load auth state instead of logging in?

---

## Emergency: Everything is Broken

If many tests suddenly fail:

1. **Check if app changed**
   - Did a new build deploy?
   - UI changes?
   
2. **Check if test environment changed**
   - Database wiped?
   - Configuration changed?
   
3. **Check if one failure cascades**
   - If auth breaks, all tests fail
   - Fix the root cause first

**Tell me:**
- What changed recently?
- Error messages from multiple tests
- I'll help identify the pattern

---

## Ready to Start

**Your first command should be:**
```
"Automate test case 2813"
```

And we're off! 🚀
