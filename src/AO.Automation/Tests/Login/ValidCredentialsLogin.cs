using System.Text.RegularExpressions;
using AO.Automation.BaseClasses;
using AO.Automation.Pages;
using Microsoft.Playwright;

namespace AO.Automation.Tests.Login;

/// <summary>
/// TC25057: Valid Credentials Login
/// Verifies user can successfully login with valid credentials and access the application
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
public class ValidCredentialsLogin : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ValidCredentialsLogin(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public async Task CanLoginWithValidCredentials()
    {
        // Persona user: automation.teammember1 (User 9100)
        // TeamMember with All Access role, redirects to /rtm on login
        // AD: Step 1 - Navigate to login and enter valid credentials
        
        var loginPage = new LoginPage(Page);
        
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember1@activeops.com", "Workware@1");
        
        // Verify: Login successful, redirects to RTM page (default for TeamMember with RTM permissions)
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
