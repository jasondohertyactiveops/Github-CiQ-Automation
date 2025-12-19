# Code Patterns & Examples

This document provides reference implementations for common patterns in the test suite.

---

## Table of Contents

1. [Test Class Patterns](#test-class-patterns)
2. [Page Object Patterns](#page-object-patterns)
3. [Component Patterns](#component-patterns)
4. [Helper Patterns](#helper-patterns)
5. [Common Scenarios](#common-scenarios)

---

## Test Class Patterns

### Pattern A: Single Comprehensive Test (Workflow)

Use for: Tests with dependent steps where state matters.

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace PlaywrightTests.Tests.Teams;

/// <summary>
/// Azure Test Case: 12345
/// Create and configure a new team with staff assignments
/// </summary>
[Trait("Suite", "Regression")]
[Trait("Feature", "Teams")]
public class CreateAndConfigureTeam : PlaywrightTest
{
    [Fact]
    public async Task CanCreateTeamAndAssignStaff()
    {
        // Arrange - Load auth state and navigate
        var teamsPage = new TeamsPage(Page);
        await Page.GotoAsync("https://test.controliq.com/teams");
        
        // Act - Create team
        await teamsPage.ClickCreateTeamButton();
        await teamsPage.FillTeamName("Test Team Alpha");
        await teamsPage.SelectTimezone("Europe/London");
        await teamsPage.ClickSaveButton();
        
        // Assert - Team created
        await Expect(Page.GetByText("Team created successfully")).ToBeVisibleAsync();
        
        // Act - Navigate to team and assign staff
        await teamsPage.SearchForTeam("Test Team Alpha");
        await teamsPage.ClickTeam("Test Team Alpha");
        await teamsPage.ClickAssignStaffButton();
        await teamsPage.SelectStaff("John Smith");
        await teamsPage.SetAssignmentDate(DateTime.Today);
        await teamsPage.ClickConfirmAssignment();
        
        // Assert - Staff assigned
        await Expect(Page.GetByText("John Smith")).ToBeVisibleAsync();
        var assignedStaff = await teamsPage.GetAssignedStaffCount();
        Assert.Equal(1, assignedStaff);
    }
}
```

### Pattern C: Multiple Tests with Shared Setup (Validation)

Use for: Independent validation checks on the same page.

```csharp
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Xunit;

namespace PlaywrightTests.Tests.Login;

/// <summary>
/// Azure Test Case: 2813
/// Verify that the Login screen shows as expected
/// </summary>
[Trait("Suite", "Smoke")]
[Trait("Suite", "Regression")]
[Trait("Feature", "Login")]
public class LoginScreenValidation : PlaywrightTest, IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        // Navigate once, all tests share this page load
        await Page.GotoAsync("https://test.controliq.com");
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task CoreElementsArePresent()
    {
        // Check essential login elements exist
        await Expect(Page.Locator(".controliq-logo")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Username")).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Password")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Login" })).ToBeVisibleAsync();
    }
    
    [Fact]
    public async Task UsernameFieldIsEmpty()
    {
        var usernameField = Page.GetByLabel("Username");
        await Expect(usernameField).ToBeEmptyAsync();
    }
    
    [Fact]
    public async Task PasswordFieldIsEmpty()
    {
        var passwordField = Page.GetByLabel("Password");
        await Expect(passwordField).ToBeEmptyAsync();
        await Expect(passwordField).ToHaveAttributeAsync("type", "password");
    }
    
    [Fact]
    public async Task LoginButtonIsEnabled()
    {
        var loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        await Expect(loginButton).ToBeEnabledAsync();
    }
    
    [Fact]
    public async Task PasswordToggleIconShowsCorrectTooltip()
    {
        var eyeIcon = Page.Locator(".password-toggle-icon");
        await Expect(eyeIcon).ToBeVisibleAsync();
        await Expect(eyeIcon).ToHaveAttributeAsync("title", "Show password");
    }
    
    [Fact]
    public async Task SSOElementsShowWhenEnabled()
    {
        // Conditional check - SSO may not be enabled in all environments
        var ssoButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login with Microsoft" });
        
        if (await ssoButton.IsVisibleAsync())
        {
            await Expect(ssoButton).ToBeVisibleAsync();
            await Expect(Page.Locator("text=-OR-")).ToBeVisibleAsync();
        }
    }
}
```

---

## Page Object Patterns

### Simple Page Object

Use for: Straightforward pages with basic interactions.

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Pages;

public class LoginPage
{
    private readonly IPage _page;
    
    // Locators as properties (lazy evaluation)
    private ILocator UsernameField => _page.GetByLabel("Username");
    private ILocator PasswordField => _page.GetByLabel("Password");
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ErrorMessage => _page.Locator(".error-message");
    
    public LoginPage(IPage page)
    {
        _page = page;
    }
    
    // High-level actions
    public async Task LoginAsync(string username, string password)
    {
        await UsernameField.FillAsync(username);
        await PasswordField.FillAsync(password);
        await LoginButton.ClickAsync();
    }
    
    public async Task<string> GetErrorMessageAsync()
    {
        return await ErrorMessage.TextContentAsync() ?? string.Empty;
    }
    
    public async Task ClickResetPasswordLinkAsync()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Reset My Password" }).ClickAsync();
    }
}
```

### Complex Page Object with Navigation

Use for: Pages with complex workflows and navigation.

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Pages;

public class TeamsPage
{
    private readonly IPage _page;
    
    // Navigation
    private ILocator CreateTeamButton => _page.GetByRole(AriaRole.Button, new() { Name = "Create Team" });
    private ILocator SearchBox => _page.GetByPlaceholder("Search teams...");
    
    // Form fields
    private ILocator TeamNameField => _page.GetByLabel("Team Name");
    private ILocator TimezoneDropdown => _page.GetByLabel("Timezone");
    private ILocator SaveButton => _page.GetByRole(AriaRole.Button, new() { Name = "Save" });
    
    public TeamsPage(IPage page)
    {
        _page = page;
    }
    
    // Navigation actions
    public async Task NavigateAsync()
    {
        await _page.GotoAsync("/teams");
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    // Creation workflow
    public async Task ClickCreateTeamButton()
    {
        await CreateTeamButton.ClickAsync();
        // Wait for modal/form to appear
        await _page.GetByRole(AriaRole.Dialog).WaitForAsync();
    }
    
    public async Task FillTeamName(string teamName)
    {
        await TeamNameField.FillAsync(teamName);
    }
    
    public async Task SelectTimezone(string timezone)
    {
        await TimezoneDropdown.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = timezone }).ClickAsync();
    }
    
    public async Task ClickSaveButton()
    {
        await SaveButton.ClickAsync();
        // Wait for save to complete
        await _page.WaitForResponseAsync(resp => 
            resp.Url.Contains("/api/teams") && resp.Status == 200);
    }
    
    // Search and selection
    public async Task SearchForTeam(string teamName)
    {
        await SearchBox.FillAsync(teamName);
        // Wait for search results to filter
        await _page.WaitForTimeoutAsync(500);
    }
    
    public async Task ClickTeam(string teamName)
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = teamName }).ClickAsync();
    }
    
    // Data retrieval
    public async Task<int> GetAssignedStaffCount()
    {
        var staffRows = _page.Locator(".staff-list .staff-row");
        return await staffRows.CountAsync();
    }
}
```

---

## Component Patterns

### Reusable Grid Component

Use for: Complex controls used across multiple pages.

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Components;

public class DataGrid
{
    private readonly IPage _page;
    private readonly string _gridSelector;
    
    private ILocator Grid => _page.Locator(_gridSelector);
    private ILocator Rows => Grid.Locator(".grid-row");
    
    public DataGrid(IPage page, string gridSelector = ".data-grid")
    {
        _page = page;
        _gridSelector = gridSelector;
    }
    
    // Grid interactions
    public async Task<int> GetRowCount()
    {
        return await Rows.CountAsync();
    }
    
    public async Task ClickRow(int rowIndex)
    {
        await Rows.Nth(rowIndex).ClickAsync();
    }
    
    public async Task<string> GetCellValue(int rowIndex, string columnName)
    {
        var cell = Rows.Nth(rowIndex).Locator($"[data-column='{columnName}']");
        return await cell.TextContentAsync() ?? string.Empty;
    }
    
    public async Task FilterByColumn(string columnName, string filterValue)
    {
        var filterButton = Grid.Locator($"[data-filter-column='{columnName}']");
        await filterButton.ClickAsync();
        
        var filterInput = _page.GetByPlaceholder($"Filter {columnName}...");
        await filterInput.FillAsync(filterValue);
        
        // Wait for grid to update
        await _page.WaitForTimeoutAsync(500);
    }
    
    public async Task SortByColumn(string columnName)
    {
        var columnHeader = Grid.Locator($"[data-column-header='{columnName}']");
        await columnHeader.ClickAsync();
        
        // Wait for sort to complete
        await _page.WaitForTimeoutAsync(300);
    }
}
```

### Capacity Planning Grid Component

Use for: Specialized complex control with drag-drop.

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Components;

public class CapacityPlanningGrid
{
    private readonly IPage _page;
    
    private ILocator Grid => _page.Locator(".capacity-grid");
    
    public CapacityPlanningGrid(IPage page)
    {
        _page = page;
    }
    
    public async Task DragWorkItemToCell(string workItemName, string staffName, DateTime date)
    {
        // Locate the work item in the backlog
        var workItem = _page.GetByText(workItemName).First;
        
        // Locate the target cell
        var targetCell = Grid.Locator(
            $"[data-staff='{staffName}'][data-date='{date:yyyy-MM-dd}']");
        
        // Perform drag and drop
        await workItem.DragToAsync(targetCell);
        
        // Wait for capacity update
        await _page.WaitForResponseAsync(resp => 
            resp.Url.Contains("/api/capacity/assign"));
    }
    
    public async Task<string> GetCapacityStatus(string staffName, DateTime date)
    {
        var cell = Grid.Locator(
            $"[data-staff='{staffName}'][data-date='{date:yyyy-MM-dd}']");
        
        var statusBadge = cell.Locator(".capacity-status");
        return await statusBadge.GetAttributeAsync("data-status") ?? "unknown";
    }
    
    public async Task FilterByTeam(string teamName)
    {
        var teamFilter = _page.GetByLabel("Team");
        await teamFilter.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = teamName }).ClickAsync();
        
        // Wait for grid to reload
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
}
```

---

## Helper Patterns

### Authentication Helper

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Helpers;

public static class AuthHelper
{
    public static async Task<IBrowserContext> CreateAuthenticatedContext(
        IBrowser browser, 
        string authStateFile)
    {
        var contextOptions = new BrowserNewContextOptions
        {
            StorageStatePath = authStateFile
        };
        
        return await browser.NewContextAsync(contextOptions);
    }
    
    public static async Task SaveAuthState(
        IPage page, 
        string username, 
        string password, 
        string outputFile)
    {
        // Perform login
        await page.GotoAsync("https://test.controliq.com");
        await page.GetByLabel("Username").FillAsync(username);
        await page.GetByLabel("Password").FillAsync(password);
        await page.GetByRole(AriaRole.Button, new() { Name = "Login" }).ClickAsync();
        
        // Wait for successful login (dashboard visible)
        await page.WaitForURLAsync("**/dashboard");
        
        // Save auth state
        await page.Context.StorageStateAsync(new() { Path = outputFile });
    }
}
```

### Workgroup Helper

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Helpers;

public class WorkgroupHelper
{
    private readonly IPage _page;
    
    public WorkgroupHelper(IPage page)
    {
        _page = page;
    }
    
    public async Task SwitchWorkgroup(string workgroupName)
    {
        // Open workgroup selector
        await _page.GetByLabel("Current Workgroup").ClickAsync();
        
        // Select workgroup
        await _page.GetByRole(AriaRole.Option, new() { Name = workgroupName }).ClickAsync();
        
        // Wait for page to reload with new workgroup context
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    public async Task<string> GetCurrentWorkgroup()
    {
        var workgroupLabel = _page.GetByLabel("Current Workgroup");
        return await workgroupLabel.TextContentAsync() ?? string.Empty;
    }
}
```

### Wait Helper

```csharp
using Microsoft.Playwright;

namespace PlaywrightTests.Helpers;

public static class WaitHelper
{
    public static async Task WaitForSignalRNotification(
        IPage page, 
        string notificationText, 
        int timeoutMs = 10000)
    {
        var notification = page.Locator(".notification-toast")
            .Filter(new() { HasText = notificationText });
        
        await notification.WaitForAsync(new() { Timeout = timeoutMs });
    }
    
    public static async Task WaitForLoadingToComplete(IPage page)
    {
        // Wait for any loading spinners to disappear
        var spinner = page.Locator(".loading-spinner");
        await spinner.WaitForAsync(new() { State = WaitForSelectorState.Hidden });
    }
    
    public static async Task WaitForApiResponse(
        IPage page, 
        string urlPattern, 
        int statusCode = 200)
    {
        await page.WaitForResponseAsync(resp =>
            resp.Url.Contains(urlPattern) && resp.Status == statusCode);
    }
}
```

---

## Common Scenarios

### Scenario: Login with Auth State

```csharp
[Fact]
public async Task TestWithAdminUser()
{
    // Load saved auth state
    var context = await Browser.NewContextAsync(new()
    {
        StorageStatePath = "Fixtures/auth-admin.json"
    });
    
    var page = await context.NewPageAsync();
    
    // Already authenticated, go straight to feature
    await page.GotoAsync("https://test.controliq.com/capacity-planning");
    
    // Test your feature
    // ...
}
```

### Scenario: Waiting for Dynamic Content

```csharp
[Fact]
public async Task TestDynamicDataLoad()
{
    await Page.GotoAsync("https://test.controliq.com/manage-data");
    
    // Wait for data to load
    await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    
    // Or wait for specific element
    await Page.Locator(".data-grid .row").First.WaitForAsync();
    
    // Now interact with data
    var rowCount = await Page.Locator(".data-grid .row").CountAsync();
    Assert.True(rowCount > 0);
}
```

### Scenario: Handling Conditional Elements

```csharp
[Fact]
public async Task TestConditionalSSO()
{
    await Page.GotoAsync("https://test.controliq.com");
    
    var ssoButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login with Microsoft" });
    
    // Check if SSO is enabled for this environment
    if (await ssoButton.IsVisibleAsync())
    {
        // Test SSO flow
        await ssoButton.ClickAsync();
        // ... SSO assertions
    }
    else
    {
        // Test username/password flow
        await Page.GetByLabel("Username").FillAsync("user@test.com");
        // ... standard login assertions
    }
}
```

### Scenario: Multi-Step Workflow

```csharp
[Fact]
public async Task TestEndToEndWorkflow()
{
    // Step 1: Navigate and switch context
    var workgroupHelper = new WorkgroupHelper(Page);
    await Page.GotoAsync("https://test.controliq.com/teams");
    await workgroupHelper.SwitchWorkgroup("Operations Team");
    
    // Step 2: Create team
    var teamsPage = new TeamsPage(Page);
    await teamsPage.ClickCreateTeamButton();
    await teamsPage.FillTeamName("Test Team");
    await teamsPage.ClickSaveButton();
    
    // Step 3: Wait for success notification
    await WaitHelper.WaitForSignalRNotification(Page, "Team created successfully");
    
    // Step 4: Navigate to capacity planning
    await Page.GotoAsync("https://test.controliq.com/capacity-planning");
    
    // Step 5: Verify team appears in capacity view
    var grid = new CapacityPlanningGrid(Page);
    await grid.FilterByTeam("Test Team");
    
    var teamHeader = Page.GetByText("Test Team");
    await Expect(teamHeader).ToBeVisibleAsync();
}
```

### Scenario: Testing with Seeded Data

```csharp
[Fact]
public async Task TestWithKnownSeededData()
{
    // Pre-seeded data: Team "Alpha Team" with ID 550e8400-e29b-41d4-a716-446655440000
    await Page.GotoAsync("https://test.controliq.com/teams");
    
    var teamsPage = new TeamsPage(Page);
    await teamsPage.SearchForTeam("Alpha Team");
    await teamsPage.ClickTeam("Alpha Team");
    
    // Verify seeded team properties
    var teamName = await Page.Locator(".team-name").TextContentAsync();
    Assert.Equal("Alpha Team", teamName);
    
    var staffCount = await teamsPage.GetAssignedStaffCount();
    Assert.Equal(5, staffCount); // Seeded with 5 staff members
}
```

---

## Anti-Patterns (Don't Do This)

### ❌ Hard-Coded Waits
```csharp
// BAD
await Page.WaitForTimeoutAsync(5000);
await Page.ClickAsync(".button");

// GOOD
await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
await Page.GetByRole(AriaRole.Button).ClickAsync();
```

### ❌ Fragile CSS Selectors
```csharp
// BAD
await Page.Locator("div > div > button.btn.btn-primary.mt-2").ClickAsync();

// GOOD
await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
```

### ❌ No Error Handling
```csharp
// BAD
var text = await element.TextContentAsync();
var count = int.Parse(text);

// GOOD
var text = await element.TextContentAsync() ?? "0";
var count = int.TryParse(text, out var result) ? result : 0;
```

### ❌ Tests Depending on Each Other
```csharp
// BAD
public class TeamTests
{
    private static string _createdTeamId;
    
    [Fact, Priority(1)]
    public async Task CreateTeam() 
    { 
        // Creates team, stores ID
        _createdTeamId = "...";
    }
    
    [Fact, Priority(2)]
    public async Task EditTeam() 
    { 
        // Uses _createdTeamId from previous test
    }
}

// GOOD
[Fact]
public async Task CanEditTeam()
{
    // Use pre-seeded team, or create within this test
    var seededTeamId = "550e8400-e29b-41d4-a716-446655440000";
    // ... test editing
}
```

---

## Tips for Writing Good Tests

1. **Use semantic locators** - GetByRole, GetByLabel, GetByText
2. **Let Playwright wait** - Use auto-waits, avoid TimeoutAsync
3. **One assertion per test** - Or closely related assertions
4. **Descriptive test names** - CanCreateTeamWithValidData not Test1
5. **Independent tests** - No shared state, use seeded data
6. **Page objects for reuse** - Extract common actions
7. **Helpers for utilities** - Don't repeat auth, waits, etc.
8. **Comments for complex logic** - Explain why, not what

---

## Quick Reference

| Need to... | Use... | Example |
|------------|--------|---------|
| Click a button | GetByRole | `GetByRole(AriaRole.Button, new() { Name = "Save" })` |
| Fill a form field | GetByLabel | `GetByLabel("Username").FillAsync("user")` |
| Find text | GetByText | `GetByText("Welcome").IsVisibleAsync()` |
| Complex selector | Locator | `Locator(".capacity-grid .row[data-id='123']")` |
| Wait for API | WaitForResponseAsync | `WaitForResponseAsync(r => r.Url.Contains("/api/teams"))` |
| Check visibility | Expect | `Expect(element).ToBeVisibleAsync()` |
| Get text | TextContentAsync | `var text = await element.TextContentAsync()` |

---

## Need More Examples?

Ask in chat:
- "Show me an example of [specific scenario]"
- "How do I test [specific feature]?"
- "Is there a pattern for [specific interaction]?"
