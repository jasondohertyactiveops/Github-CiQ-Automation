using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.UI.Client.Pages.MyAccount;
using AO.Automation.UI.Client.Pages.Shared;
using AO.Automation.Shared.Attributes;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Admin.Account.MyAccount;

[AzureTestSuite(25146)] // Login
[AzureTestCase(29202)]
[AzureTestPlan("Smoke")]
[AzureTestPlan("Regression")]
public class ChangePasswordFromMyAccount : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ChangePasswordFromMyAccount(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    [AzureTestStep(29202, 1)]
    [Trait("Category", "OneShot")]
    public async Task CanChangePasswordAndLoginWithNewPassword()
    {
        // Seeded user: tc29202.passwordchange@activeops.com (User 9007)
        // Current password: Workware@1
        // AD: Step 1 - Login and navigate to My Account
        
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("tc29202.passwordchange@activeops.com", "Workware@1");
        
        var userMenu = new UserMenuComponent(Page);
        await userMenu.NavigateToMyAccountAsync();
        
        // AD: Step 2 - Open Change Password dialog
        var myAccountPage = new MyAccountPage(Page);
        await myAccountPage.ClickChangePasswordButtonAsync();
        
        // AD: Step 3 - Change password
        var changePasswordDialog = new ChangePasswordDialog(Page);
        var newPassword = "NewPass@456";
        await changePasswordDialog.ChangePasswordAsync("Workware@1", newPassword);
        
        // Verify: Password change successful
        Assert.True(await changePasswordDialog.IsChangeSuccessfulAsync(), 
            "Password change should show success message");
        
        // AD: Step 4 - Logout to test new password
        await Page.GotoAsync("/logout");
        await Page.WaitForURLAsync(new Regex("^http://ww7client\\.localhost/?$"));
        
        // AD: Step 5 - Login with NEW password (already on login page after logout)
        await loginPage.LoginAsync("tc29202.passwordchange@activeops.com", newPassword);
        
        // Verify: Login successful with new password
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
