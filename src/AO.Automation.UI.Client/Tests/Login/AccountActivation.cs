using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.Shared.Helpers;
using AO.Automation.UI.Client.Pages.Login;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

/// <summary>
/// TC24166: Account Activation
/// Verifies user can activate account using activation link and login for first time
/// NOTE: One-shot test - activates User 9003 permanently. Requires fresh database to re-run.
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("Category", "OneShot")]
public class AccountActivation : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public AccountActivation(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public async Task CanActivateAccountAndLoginForFirstTime()
    {
        // Seeded data: User 9003 in "Invited" status (ActivationStatusId = 2)
        // SecurityStamp: 3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D
        // AD: Step 1 - Generate activation token and navigate to activation page
        
        var tokenHelper = new TokenHelper(Config.JwtActivationKey, Config.JwtResetPasswordKey);
        var activationToken = tokenHelper.GenerateActivationToken(
            clientIdentifier: "ww7client",
            staffMemberId: 9003,
            email: "tc24166.activation@activeops.com",
            securityStamp: "3A1B2C3D-4E5F-6A7B-8C9D-0E1F2A3B4C5D"
        );
        
        var activationPage = new ActivationPage(Page);
        await activationPage.NavigateAsync(activationToken);
        
        // Verify: Activation form displays with pre-filled username
        var username = await activationPage.GetUsernameAsync();
        Assert.Equal("tc24166.activation@activeops.com", username);
        
        // AD: Step 2 - Set password and submit activation
        var newPassword = "NewPassword@123";
        await activationPage.ActivateAccountAsync(newPassword);
        
        // Verify: Activation successful
        Assert.True(await activationPage.IsActivationSuccessfulAsync(), 
            "Account activation should show success message");
        
        // AD: Step 3 - Navigate to login screen
        await activationPage.GoToLoginAsync();
        
        // AD: Step 4 - Login with newly activated credentials
        var loginPage = new LoginPage(Page);
        await loginPage.LoginAsync("tc24166.activation@activeops.com", newPassword);
        
        // Verify: First login successful, redirects to RTM (user has All Access with RTM permissions)
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
