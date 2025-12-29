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
    private ILocator ErrorMessage => _page.Locator("[role='alert']");
    
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
    }
    
    /// <summary>
    /// Get error message text if visible
    /// </summary>
    public async Task<string?> GetErrorMessageAsync()
    {
        if (await ErrorMessage.IsVisibleAsync())
        {
            return await ErrorMessage.TextContentAsync();
        }
        return null;
    }
}
