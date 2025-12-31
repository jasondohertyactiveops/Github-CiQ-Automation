using AO.Automation.BaseClasses;
using AO.Automation.Pages;
using Microsoft.Playwright;

namespace AO.Automation.Tests.Login;

/// <summary>
/// TC25058: Invalid Credentials Login
/// Verifies login fails appropriately with invalid credentials and shows correct error messages
/// NOTE: OneShot tests - lock accounts on failure. Requires fresh database to re-run.
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("Category", "OneShot")]
public class InvalidCredentialsLogin : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public InvalidCredentialsLogin(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
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
