using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Pages.MyAccount;

/// <summary>
/// Component for General Preferences tab on My Account page
/// </summary>
public class GeneralPreferencesTab
{
    private readonly IPage _page;
    
    private ILocator Tab => _page.Locator("[data-tag='generalpreferences-tab']");
    private ILocator LanguageDropdown => _page.Locator("[data-tag='select'] [data-tag='dropdown']");
    
    public GeneralPreferencesTab(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Click General Preferences tab
    /// </summary>
    public async Task NavigateToTabAsync()
    {
        await Tab.ClickAsync();
    }
    
    /// <summary>
    /// Select language from dropdown
    /// </summary>
    public async Task SelectLanguageAsync(string language)
    {
        await LanguageDropdown.ClickAsync();
        await _page.GetByRole(AriaRole.Option, new() { Name = language }).ClickAsync();
        
        // Wait for API call to save preference
        await _page.WaitForResponseAsync(response => 
            response.Url.Contains("/api/UserPreference/") && 
            response.Url.Contains("/localisation") &&
            response.Request.Method == "PUT");
        
        // Wait for page to re-render with new language
        await _page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle);
        await _page.WaitForTimeoutAsync(1000); // Extra time for React to update all labels
    }
}
