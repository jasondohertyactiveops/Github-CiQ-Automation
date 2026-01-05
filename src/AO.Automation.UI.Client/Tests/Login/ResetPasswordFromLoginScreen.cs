using System.Text.RegularExpressions;
using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.Shared.Helpers;
using AO.Automation.UI.Client.Pages.Login;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Login;

/// <summary>
/// TC25061: Reset Password from Login Screen
/// Verifies user can reset password using reset link and login with new password
/// NOTE: OneShot test - changes User 9006 password permanently. Requires fresh database to re-run.
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "Login")]
[Trait("Category", "OneShot")]
public class ResetPasswordFromLoginScreen : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ResetPasswordFromLoginScreen(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public async Task CanResetPasswordAndLoginWithNewPassword()
    {
        // Seeded data: User 9006 (tc25061.reset@activeops.com)
        // Initial password: Workware@1
        // SecurityStamp: 4B2C3D4E-5F6A-7B8C-9D0E-1F2A3B4C5D6E
        // AD: Step 1 - Generate reset token and navigate to reset page
        
        var tokenService = new TokenService(Config.JwtActivationKey, Config.JwtResetPasswordKey);
        var resetToken = tokenService.GenerateResetPasswordToken(
            clientIdentifier: "ww7client",
            staffMemberId: 9006,
            username: "tc25061.reset@activeops.com",
            securityStamp: "4B2C3D4E-5F6A-7B8C-9D0E-1F2A3B4C5D6E"
        );
        
        var resetPage = new ResetPasswordPage(Page);
        await resetPage.NavigateAsync(resetToken);
        
        // AD: Step 2 - Set new password and submit (8-15 chars, mixed case, number, symbol)
        var newPassword = "NewPass@123";
        await resetPage.ResetPasswordAsync(newPassword);
        
        // Verify: Reset successful
        Assert.True(await resetPage.IsResetSuccessfulAsync(), 
            "Password reset should show success message");
        
        // AD: Step 3 - Navigate to login screen
        await resetPage.GoToLoginAsync();
        
        // AD: Step 4 - Login with NEW password
        var loginPage = new LoginPage(Page);
        await loginPage.LoginAsync("tc25061.reset@activeops.com", newPassword);
        
        // Verify: Login successful with new password, redirects to RTM
        await Expect(Page).ToHaveURLAsync(new Regex("/rtm"));
    }
}
