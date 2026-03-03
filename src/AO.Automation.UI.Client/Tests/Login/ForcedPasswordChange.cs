using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.Shared.Attributes;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

[AzureTestSuite(25146)] // Login
[AzureTestCase(29201)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ForcedPasswordChange : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ForcedPasswordChange(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    [AzureTestStep(29201, 1)]
    [Trait("Category", "OneShot")]
    public async Task UserForcedToChangePasswordAfterAdminReset()
    {
        // Seeded user: tc29201.mustchange@activeops.com (User 9008)
        // Temp password: Workware@1
        // ChangePasswordOnLogin = 1 (simulates admin having reset password)
        // AD: Step 1 - Attempt login with temp password
        
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        
        // Login will get 403 and redirect to /changepassword
        await loginPage.LoginAsync("tc29201.mustchange@activeops.com", "Workware@1");
        
        // Verify: Redirected to change password screen (NOT dashboard)
        await Expect(Page).ToHaveURLAsync(new Regex("/changepassword"));
        
        // AD: Step 2 - Change password on forced change screen
        // Username should be pre-filled and read-only
        var username = await Page.GetByLabel("Username").InputValueAsync();
        Assert.Equal("tc29201.mustchange@activeops.com", username);
        
        // Fill password change form
        await Page.GetByLabel("Current Password").FillAsync("Workware@1");
        await Page.GetByLabel("New Password", new() { Exact = true }).FillAsync("NewPass@456");
        await Page.GetByLabel("Confirm New Password").FillAsync("NewPass@456");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
        
        // Wait for redirect or success (might go to login or straight to app)
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // AD: Step 3 - After submitting forced password change, should auto-login or redirect to login
        // The test saw URLs: /changepassword → / (root)
        // Need to handle redirect to root and login again
        var currentUrl = Page.Url;
        
        // Always login again after forced password change (safer than checking URL)
        if (!currentUrl.Contains("/rtm"))
        {
            await Page.WaitForURLAsync(new Regex("^http://ww7client\\.localhost/?$"));
            await loginPage.LoginAsync("tc29201.mustchange@activeops.com", "NewPass@456");
        }
        
        // Verify: Successfully logged in, at RTM dashboard
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
        
        // Verify: No forced password change on subsequent access (flag cleared)
        await Page.GotoAsync("/logout");
        await Page.WaitForURLAsync(new Regex("^http://ww7client\\.localhost/?$"));
        
        await loginPage.LoginAsync("tc29201.mustchange@activeops.com", "NewPass@456");
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm")); // Should go straight to RTM, not /changepassword
    }
}
