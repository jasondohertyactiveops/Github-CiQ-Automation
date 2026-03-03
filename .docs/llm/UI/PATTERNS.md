# UI Code Patterns

Reference implementations for common patterns.

---

## Test Class Patterns

### Pattern A: Workflow Test

Dependent steps where state matters.

```csharp
[AzureTestSuite(25146)] // Login
[AzureTestCase(25057)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ValidCredentialsLogin : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ValidCredentialsLogin(BrowserFixture browserFixture) : base(browserFixture) { }

    [Fact]
    [AzureTestStep(25057, 1)]
    public async Task CanLoginWithValidCredentials()
    {
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember1@activeops.com", "Workware@1");
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
```

### Pattern C: Multiple Tests with Shared Fixture

Independent validation checks on the same page.

```csharp
[AzureTestSuite(25146)] // Login
[AzureTestCase(24230)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ViewMyAccountDetails : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ViewMyAccountDetails(BrowserFixture browserFixture) : base(browserFixture) { }

    [Fact]
    [AzureTestStep(24230, 1)]
    public async Task UserMenuShowsCorrectDetails() { /* ... */ }

    [Fact]
    [AzureTestStep(24230, 2)]
    public async Task MyAccountPageShowsCorrectEmployeeDetails() { /* ... */ }

    [Fact]
    [AzureTestStep(24230, 3)]
    public async Task MyAccountPageShowsCorrectLoginDetails() { /* ... */ }
}
```

### OneShot Test

Mark on method, never on class.

```csharp
[Fact]
[AzureTestStep(24166, 1)]
[Trait("Category", "OneShot")]
public async Task CanActivateAccountAndLoginForFirstTime() { /* ... */ }
```

---

## Page Object Pattern

```csharp
public class LoginPage
{
    private readonly IPage _page;

    private ILocator UsernameField => _page.GetByLabel("Username");
    private ILocator PasswordField => _page.GetByLabel("Password");
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ErrorMessage => _page.Locator(".error-message");

    public LoginPage(IPage page) { _page = page; }

    public async Task NavigateAsync() => await _page.GotoAsync("/");

    public async Task LoginAsync(string username, string password)
    {
        await UsernameField.FillAsync(username);
        await PasswordField.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task<string?> GetErrorMessageAsync() => await ErrorMessage.TextContentAsync();
}
```

---

## Locator Hierarchy

```csharp
// 1. Semantic (preferred)
await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
await Page.GetByLabel("Username").FillAsync("user");
await Page.GetByText("Welcome").IsVisibleAsync();

// 2. CSS (when semantic doesn't work)
await Page.Locator(".capacity-grid .row[data-id='123']").ClickAsync();

// 3. Test ID (last resort only)
await Page.GetByTestId("complex-widget").ClickAsync();
```

---

## Infrastructure

**BrowserFixture:** Shares browser instance per test class. Each test gets isolated Context and Page.

**PlaywrightTest base class provides:** `Browser`, `Context`, `Page`, `Config`, automatic cleanup.

**Shared project (AO.Automation.Shared):** TokenHelper for JWT generation, traceability attributes, test config base class.

---

## Anti-Patterns

```csharp
// ❌ Hard-coded waits
await Page.WaitForTimeoutAsync(5000);

// ✅ Let Playwright auto-wait
await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);

// ❌ Fragile CSS selectors
await Page.Locator("div > div > button.btn.btn-primary.mt-2").ClickAsync();

// ✅ Semantic locator
await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();

// ❌ Tests depending on each other via shared state
private static string _createdId;

// ✅ Each test uses pre-seeded data or creates its own
var seededTeamId = "550e8400-...";
```
