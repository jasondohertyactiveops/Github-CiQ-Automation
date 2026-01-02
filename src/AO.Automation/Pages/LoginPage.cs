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
        
        // After login, wait for redirect and close RTM dialog if present
        try
        {
            await _page.WaitForURLAsync(new System.Text.RegularExpressions.Regex("/rtm"), new() { Timeout = 5000 });
            
            // Wait for RTM dialog to appear and close it
            await _page.WaitForTimeoutAsync(1000);
            var closeButton = _page.GetByTestId("close-btn");
            if (await closeButton.IsVisibleAsync())
            {
                await closeButton.ClickAsync();
                await _page.WaitForTimeoutAsync(500);
            }
        }
        catch
        {
            // Not redirecting to /rtm or dialog not present
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
