using AO.Automation.Pages;
using System.Text.RegularExpressions;

namespace AO.Automation.Tests.Login;

/// <summary>
/// Azure Test Case: 2813
/// Verify that the Login screen shows as expected
/// </summary>
[Trait("Suite", "Smoke")]
[Trait("Suite", "Regression")]
[Trait("Feature", "Login")]
public class LoginScreenValidation : PlaywrightTest, IClassFixture<BrowserFixture>
{
    private LoginPage _loginPage = null!;
    
    public LoginScreenValidation(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        // Step 1: Launch the ControliQ Application using the URL
        _loginPage = new LoginPage(Page);
        await _loginPage.NavigateAsync();
    }
    
    [Fact]
    public async Task PageTitleShowsLogIn()
    {
        // Verify page title
        await Expect(Page).ToHaveTitleAsync(new Regex("Log in|Login|ControliQ"));
    }
    
    [Fact]
    public async Task ControliQLogoIsVisible()
    {
        // Step 2: The main ControliQ logo
        var logo = Page.GetByAltText("Click here to go to the ActiveOps website");
        await Expect(logo).ToBeVisibleAsync();
    }
    
    [Fact]
    public async Task ActiveOpsLinkExistsWithCorrectBehavior()
    {
        // Step 2: 'ControliQ' logo with hover-over message and link to activeops.com
        var activeOpsLink = Page.GetByRole(AriaRole.Link, new() { Name = "Click here to go to the ActiveOps website" });
        await Expect(activeOpsLink).ToBeVisibleAsync();
        await Expect(activeOpsLink).ToHaveAttributeAsync("href", new Regex("activeops.com"));
    }
    
    [Fact]
    public async Task LoginWithMicrosoftButtonShowsIfSSOEnabled()
    {
        // Step 2: Login with Microsoft button if client is SSO enabled
        // This is conditional - may not exist in all environments
        var ssoButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login with Microsoft" });
        
        // If SSO enabled, button should be visible
        var isVisible = await ssoButton.IsVisibleAsync();
        if (isVisible)
        {
            await Expect(ssoButton).ToBeVisibleAsync();
            // If SSO button exists, -OR- label should also exist
            await Expect(Page.Locator("text=-OR-")).ToBeVisibleAsync();
        }
    }
    
    [Fact]
    public async Task UsernameFieldIsVisibleAndEmpty()
    {
        // Step 2: A Username textbox field - empty by default
        var usernameField = Page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
        await Expect(usernameField).ToBeVisibleAsync();
        await Expect(usernameField).ToBeEmptyAsync();
    }
    
    [Fact]
    public async Task PasswordFieldIsVisibleAndEmptyWithEyeIcon()
    {
        // Step 2: A Password textbox field with an eye icon - empty by default
        var passwordField = Page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
        await Expect(passwordField).ToBeVisibleAsync();
        await Expect(passwordField).ToBeEmptyAsync();
        await Expect(passwordField).ToHaveAttributeAsync("type", "password");
        
        // Eye icon with hover text 'Show password'
        var showPasswordButton = Page.GetByRole(AriaRole.Button, new() { Name = "Show password" });
        await Expect(showPasswordButton).ToBeVisibleAsync();
    }
    
    [Fact]
    public async Task LoginButtonIsEnabledByDefault()
    {
        // Step 2: Login button - enabled by default
        // NOTE: Test case description says "disabled" but expected result says "enabled"
        var loginButton = Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        await Expect(loginButton).ToBeVisibleAsync();
        await Expect(loginButton).ToBeEnabledAsync();
    }
    
    [Fact]
    public async Task ResetMyPasswordLinkIsVisible()
    {
        // Step 2: Reset My Password link
        var resetPasswordButton = Page.GetByRole(AriaRole.Button, new() { Name = "Reset My Password" });
        await Expect(resetPasswordButton).ToBeVisibleAsync();
    }
}

