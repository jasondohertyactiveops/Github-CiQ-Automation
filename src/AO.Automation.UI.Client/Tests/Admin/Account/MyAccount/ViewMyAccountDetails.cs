using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.UI.Client.Pages.MyAccount;
using AO.Automation.UI.Client.Pages.RTM;
using AO.Automation.UI.Client.Pages.Shared;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Admin.Account.MyAccount;

/// <summary>
/// TC24230: View My Account Page
/// Verifies My Account page displays correct personal information for logged-in user
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "MyAccount")]
public class ViewMyAccountDetails : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public ViewMyAccountDetails(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public async Task UserMenuShowsCorrectDetails()
    {
        // Persona user: automation.teammember1 (User 9100)
        // AD: Step 1 - Login as test user
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember1@activeops.com", "Workware@1");
        
        // Wait for dashboard to load
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/rtm"));
        
        // Close RTM dialog if it appears (blocks interactions)
        var rtmPage = new RtmPage(Page);
        await rtmPage.CloseSelectActivityDialogIfPresentAsync();
        
        // AD: Step 2 - Check user menu icon and dropdown
        var userMenu = new UserMenuComponent(Page);
        
        // Verify: User icon shows initials (A T from "Automation TeamMember1")
        var initials = await userMenu.GetInitialsAsync();
        Assert.Equal("AT", initials?.Trim());
        
        // Open dropdown and verify details
        await userMenu.OpenMenuAsync();
        
        var fullName = await userMenu.GetFullNameFromDropdownAsync();
        Assert.Contains("Automation TeamMember1", fullName);
        
        var email = await userMenu.GetEmailFromDropdownAsync();
        Assert.Equal("automation.teammember1@activeops.com", email?.Trim());
    }
    
    [Fact]
    public async Task MyAccountPageShowsCorrectEmployeeDetails()
    {
        // AD: Step 1 - Login and navigate to My Account
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember1@activeops.com", "Workware@1");
        
        // Wait for redirect to RTM and close dialog
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/rtm"));
        var rtmPage = new RtmPage(Page);
        await rtmPage.CloseSelectActivityDialogIfPresentAsync();
        
        var userMenu = new UserMenuComponent(Page);
        await userMenu.NavigateToMyAccountAsync();
        
        // AD: Step 2 - Verify My Account page title
        var myAccountPage = new MyAccountPage(Page);
        var pageTitle = await myAccountPage.GetPageTitleAsync();
        Assert.Contains("Automation TeamMember1", pageTitle);
        
        // AD: Step 3 - Verify Employee Details card
        var userType = await myAccountPage.GetUserTypeAsync();
        Assert.Equal("Team Member", userType?.Trim());
        
        var employmentType = await myAccountPage.GetEmploymentTypeAsync();
        Assert.Equal("Full-time", employmentType?.Trim());
        
        var refId = await myAccountPage.GetRefIdAsync();
        Assert.Equal("AUTO-TM1", refId?.Trim());
        
        var location = await myAccountPage.GetLocationAsync();
        Assert.Contains("London", location); // Europe/London displayed as readable location
    }
    
    [Fact]
    public async Task MyAccountPageShowsCorrectLoginDetails()
    {
        // AD: Step 1 - Login and navigate to My Account
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember1@activeops.com", "Workware@1");
        
        // Wait for redirect to RTM and close dialog
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/rtm"));
        var rtmPage = new RtmPage(Page);
        await rtmPage.CloseSelectActivityDialogIfPresentAsync();
        
        var userMenu = new UserMenuComponent(Page);
        await userMenu.NavigateToMyAccountAsync();
        
        // AD: Step 2 - Verify Login Details card
        var myAccountPage = new MyAccountPage(Page);
        
        var username = await myAccountPage.GetUsernameAsync();
        Assert.Equal("automation.teammember1@activeops.com", username?.Trim());
        
        var changePasswordEnabled = await myAccountPage.IsChangePasswordButtonEnabledAsync();
        Assert.True(changePasswordEnabled, "Change Password button should be enabled");
    }
}
