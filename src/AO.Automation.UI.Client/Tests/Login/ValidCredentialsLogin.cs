using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.Shared.Attributes;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

[AzureTestSuite(25146)] // Login
[AzureTestCase(25057)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ValidCredentialsLogin : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ValidCredentialsLogin(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    [AzureTestStep(25057, 1)]
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
