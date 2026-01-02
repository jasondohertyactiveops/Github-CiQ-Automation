using Microsoft.Playwright;

namespace AO.Automation.Pages;

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
    public async Task LoginAsync(string username, string password)
    {
        await UsernameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
        
        // After login, wait for any redirect and close RTM dialog if we land on /rtm
        try
        {
            // Wait for any navigation to complete
            await _page.WaitForLoadStateAsync(Microsoft.Playwright.LoadState.NetworkIdle, new() { Timeout = 5000 });
            
            // If on /rtm, close the RTM dialog
            if (_page.Url.Contains("/rtm"))
            {
                // Wait for dialog to appear, then close it
                var rtmDialog = _page.GetByRole(AriaRole.Dialog, new() { Name = "Select Your Activity" });
                try
                {
                    await rtmDialog.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 3000 });
                    var closeButton = _page.GetByTestId("close-btn");
                    await closeButton.ClickAsync();
                    // Wait for dialog to fully close
                    await rtmDialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 2000 });
                }
                catch
                {
                    // Dialog didn't appear or already closed
                }
            }
        }
        catch
        {
            // Navigation didn't complete or dialog not present - continue
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
