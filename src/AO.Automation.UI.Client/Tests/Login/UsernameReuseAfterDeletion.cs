using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.Shared.Attributes;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

[AzureTestSuite(25146)] // Login
[AzureTestCase(24155)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class UsernameReuseAfterDeletion : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public UsernameReuseAfterDeletion(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    [AzureTestStep(24155, 1)]
    public async Task CanLoginWithUsernameAfterDeletionAndRecreation()
    {
        // Seeded data: tc24155.duplicate@activeops.com exists twice (User 9001 deleted, User 9002 active)
        // User 9002 has All Access role which includes RTM permissions
        // TeamMember user type with RTM permissions redirects to /rtm on login
        // AD: Step 1 - Login with duplicated username
        
        var loginPage = new LoginPage(Page);
        
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("tc24155.duplicate@activeops.com", "Workware@1");
        
        // Verify: System finds active user (9002), login succeeds, redirects to RTM page
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
