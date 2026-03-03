using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.Shared.Attributes;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

[AzureTestSuite(25146)] // Login
[AzureTestCase(25058)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class InvalidCredentialsLogin : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public InvalidCredentialsLogin(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    [AzureTestStep(25058, 1)]
    [Trait("Category", "OneShot")]
    public async Task InvalidUsernamePasswordShowsError()
    {
        // AD: Step 1 - Attempt login with invalid credentials
        var loginPage = new LoginPage(Page);
        
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("invalid.user@test.com", "WrongPassword123");
        
        // Verify: Error message shown
        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.NotNull(errorMessage);
        Assert.Contains("Sorry, your login was unsuccessful", errorMessage);
        
        // Verify: Still on login page (not authenticated)
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("^http://ww7client\\.localhost/?$"));
    }
    
    [Fact]
    [AzureTestStep(25058, 2)]
    [Trait("Category", "OneShot")]
    public async Task NoRoleAssignedShowsError()
    {
        // Seeded user: tc25058.norole@activeops.com (User 9004) - has account but no roles
        // AD: Step 1 - Attempt login with no-role user
        var loginPage = new LoginPage(Page);
        
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("tc25058.norole@activeops.com", "Workware@1");
        
        // Verify: Error message about missing roles (on fresh DB, before lockout)
        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.NotNull(errorMessage);
        Assert.Contains("You don't have any roles assigned", errorMessage);
        
        // Verify: Still on login page
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("^http://ww7client\\.localhost/?$"));
    }
    
    [Fact]
    [AzureTestStep(25058, 3)]
    [Trait("Category", "OneShot")]
    public async Task InactiveAccountShowsError()
    {
        // Seeded user: tc25058.inactive@activeops.com (User 9005) - account is inactive
        // AD: Step 1 - Attempt login with inactive user
        var loginPage = new LoginPage(Page);
        
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("tc25058.inactive@activeops.com", "Workware@1");
        
        // Verify: Error message shown (unsuccessful login)
        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.NotNull(errorMessage);
        // Error message may vary - just verify login failed
        Assert.NotEmpty(errorMessage);
        
        // Verify: Still on login page
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("^http://ww7client\\.localhost/?$"));
    }
}
