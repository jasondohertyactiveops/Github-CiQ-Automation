using System.Text.RegularExpressions;
using AO.Automation.BaseClasses;
using AO.Automation.Pages;
using Microsoft.Playwright;

namespace AO.Automation.Tests.Admin.Account.MyAccount;

/// <summary>
/// TC29202: Reset Password from My Account Page
/// Verifies user can change their own password from My Account and login with new credentials
/// NOTE: OneShot test - changes User 9007 password permanently. Requires fresh database to re-run.
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "MyAccount")]
[Trait("Category", "OneShot")]
public class ChangePasswordFromMyAccount : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ChangePasswordFromMyAccount(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
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
