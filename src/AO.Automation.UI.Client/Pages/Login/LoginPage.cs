using Microsoft.Playwright;
using AO.Automation.UI.Client.Pages.RTM;

namespace AO.Automation.UI.Client.Pages.Login;

/// <summary>
/// Page Object Model for the Login page
/// </summary>
public class LoginPage
{
    private readonly IPage _page;
    
    // Locators using semantic selectors
    private ILocator UsernameInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
    private ILocator PasswordInput => _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ErrorMessage => _page.Locator("p").Filter(new() 
        { HasTextRegex = new System.Text.RegularExpressions.Regex("Sorry, your login was unsuccessful|You don't have any roles assigned") });
    
    public LoginPage(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Navigate to the login page
    /// </summary>
    public async Task NavigateAsync()
    {
        await _page.GotoAsync("/");
    }
    
    /// <summary>
    /// Perform login with username and password
    /// </summary>
    /// <param name="username">Username to login with</param>
    /// <param name="password">Password to login with</param>
    /// <param name="keepRedirectModalOpen">If false (default), automatically closes RTM "Select Your Activity" dialog. Set to true for RTM-specific tests.</param>
    public async Task LoginAsync(string username, string password, bool keepRedirectModalOpen = false)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
        
        // Wait for navigation to /rtm if that's where we're going
        try
        {
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/rtm"), new() { Timeout = 10000 });
            
            // If we land on RTM and don't want the dialog, close it
            if (!keepRedirectModalOpen)
            {
                var rtmPage = new RtmPage(_page);
                await rtmPage.CloseSelectActivityDialogIfPresentAsync();
            }
        }
        catch (TimeoutException)
        {
            // Didn't navigate to /rtm - might be on a different page, that's fine
        }
    }
    
    /// <summary>
    /// Get error message text if visible
    /// </summary>
    public async Task<string?> GetErrorMessageAsync()
    {
        try
        {
            // Wait for error message to appear (up to 5 seconds)
            await ErrorMessage.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return await ErrorMessage.TextContentAsync();
        }
        catch
        {
            // No error message appeared
            return null;
        }
    }
}
