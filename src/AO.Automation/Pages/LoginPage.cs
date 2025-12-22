namespace AO.Automation.Pages;

/// <summary>
/// Page Object for the ControliQ Login page
/// </summary>
public class LoginPage
{
    private readonly IPage _page;
    
    // Locators - using semantic selectors based on actual page structure
    private ILocator UsernameField => _page.GetByRole(AriaRole.Textbox, new() { Name = "Username" });
    private ILocator PasswordField => _page.GetByRole(AriaRole.Textbox, new() { Name = "Password" });
    private ILocator ShowPasswordButton => _page.GetByRole(AriaRole.Button, new() { Name = "Show password" });
    private ILocator LoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Login" });
    private ILocator ResetPasswordButton => _page.GetByRole(AriaRole.Button, new() { Name = "Reset My Password" });
    private ILocator ActiveOpsLink => _page.GetByRole(AriaRole.Link, new() { Name = "Click here to go to the ActiveOps website" });
    
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
        // Wait for page to fully load
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
    }
    
    /// <summary>
    /// Perform login with username and password
    /// </summary>
    public async Task LoginAsync(string username, string password)
    {
        await UsernameField.FillAsync(username);
        await PasswordField.FillAsync(password);
        await LoginButton.ClickAsync();
    }
    
    /// <summary>
    /// Check if username field is visible
    /// </summary>
    public async Task<bool> IsUsernameFieldVisibleAsync()
    {
        return await UsernameField.IsVisibleAsync();
    }
    
    /// <summary>
    /// Check if username field is empty
    /// </summary>
    public async Task<bool> IsUsernameFieldEmptyAsync()
    {
        var value = await UsernameField.InputValueAsync();
        return string.IsNullOrEmpty(value);
    }
    
    /// <summary>
    /// Check if password field is visible
    /// </summary>
    public async Task<bool> IsPasswordFieldVisibleAsync()
    {
        return await PasswordField.IsVisibleAsync();
    }
    
    /// <summary>
    /// Check if password field is empty
    /// </summary>
    public async Task<bool> IsPasswordFieldEmptyAsync()
    {
        var value = await PasswordField.InputValueAsync();
        return string.IsNullOrEmpty(value);
    }
    
    /// <summary>
    /// Click the Reset My Password button
    /// </summary>
    public async Task ClickResetPasswordAsync()
    {
        await ResetPasswordButton.ClickAsync();
    }
    
    /// <summary>
    /// Click the ActiveOps logo/link
    /// </summary>
    public async Task ClickActiveOpsLinkAsync()
    {
        await ActiveOpsLink.ClickAsync();
    }
}
