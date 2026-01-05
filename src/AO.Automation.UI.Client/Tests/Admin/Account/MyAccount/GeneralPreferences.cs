using AO.Automation.UI.Client.BaseClasses;
using AO.Automation.UI.Client.Pages.Login;
using AO.Automation.UI.Client.Pages.MyAccount;
using AO.Automation.UI.Client.Pages.RTM;
using AO.Automation.UI.Client.Pages.Shared;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Tests.Admin.Account.MyAccount;

/// <summary>
/// TC25688: View/Update General Preferences
/// Verifies user can change language preference and UI updates accordingly
/// NOTE: OneShot test - if fails mid-test, user left in French state
/// </summary>
[Trait("Suite", "Login-25146")]
[Trait("Feature", "MyAccount")]
[Trait("Category", "OneShot")]
public class GeneralPreferences : PlaywrightTest, IClassFixture<BrowserFixture>
{
    public GeneralPreferences(BrowserFixture browserFixture) : base(browserFixture)
    {
    }
    
    [Fact]
    public async Task CanChangeLanguagePreferenceToFrenchAndBack()
    {
        // Persona user: automation.teammember2 (dedicated for language testing, no interference)
        // AD: Step 1 - Login and navigate to My Account
        
        var loginPage = new LoginPage(Page);
        await loginPage.NavigateAsync();
        await loginPage.LoginAsync("automation.teammember2@activeops.com", "Workware@1");
        
        // Wait for redirect to RTM and close dialog
        await Expect(Page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex("/rtm"));
        var rtmPage = new RtmPage(Page);
        await rtmPage.CloseSelectActivityDialogIfPresentAsync();
        
        var userMenu = new UserMenuComponent(Page);
        await userMenu.NavigateToMyAccountAsync();
        
        // AD: Step 2 - Navigate to General Preferences tab
        var preferencesTab = new GeneralPreferencesTab(Page);
        await preferencesTab.NavigateToTabAsync();
        
        // AD: Step 3 - Change language to French
        await preferencesTab.SelectLanguageAsync("French (Canada)");
        
        // Wait for page to update with French text
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // AD: Step 4 - Verify French labels (spot check)
        // Tab names
        var accountDetailsTab = await Page.Locator("[data-tag='accountdetails-tab'] h3").TextContentAsync();
        Assert.Equal("Détails du compte", accountDetailsTab);
        
        var generalPrefsTab = await Page.Locator("[data-tag='generalpreferences-tab'] h3").TextContentAsync();
        Assert.Equal("Préférences générales", generalPrefsTab);
        
        // Preferences card title
        var prefsCardTitle = await Page.Locator("[data-tag='preferences-card'] [data-tag='title']").TextContentAsync();
        Assert.Equal("Préférences", prefsCardTitle);
        
        // Switch to Account Details tab to check more labels
        await Page.Locator("[data-tag='accountdetails-tab']").ClickAsync();
        
        // Employee Details card (French)
        var employeeDetailsTitle = await Page.Locator("[data-tag='employeedetails-card'] [data-tag='title']").TextContentAsync();
        Assert.Contains("Détails de l", employeeDetailsTitle); // Partial match to avoid apostrophe encoding issues
        
        var userTypeLabel = await Page.Locator("[data-tag='employeedetails-card'] [data-tag='usertype-label']").TextContentAsync();
        Assert.Contains("Type d", userTypeLabel); // Partial match to avoid apostrophe encoding issues
        
        // AD: Step 5 - Change back to English
        await Page.Locator("[data-tag='generalpreferences-tab']").ClickAsync();
        await preferencesTab.SelectLanguageAsync("English (United Kingdom)");
        
        // Wait for page to update back to English
        await Page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        
        // AD: Step 6 - Verify English labels restored
        await Page.Locator("[data-tag='accountdetails-tab']").ClickAsync();
        
        var employeeDetailsTitleEn = await Page.Locator("[data-tag='employeedetails-card'] [data-tag='title']").TextContentAsync();
        Assert.Equal("Employee Details", employeeDetailsTitleEn);
    }
}
