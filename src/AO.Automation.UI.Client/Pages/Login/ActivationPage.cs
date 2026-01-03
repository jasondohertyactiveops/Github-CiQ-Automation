using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Pages.Login;

/// <summary>
/// Page Object Model for the Account Activation page
/// </summary>
public class ActivationPage
{
    private readonly IPage _page;
    
    // Locators using semantic selectors
    private ILocator UsernameField => _page.GetByLabel("Username");
    private ILocator NewPasswordField => _page.GetByLabel("New Password", new() { Exact = true });
    private ILocator ConfirmPasswordField => _page.GetByLabel("Confirm New Password");
    private ILocator SubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
    private ILocator SuccessMessage => _page.GetByText("Your account has been activated successfully");
    private ILocator GoToLoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Go to Login Screen" });
    
    public ActivationPage(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Navigate to the activation page with token
    /// </summary>
    public async Task NavigateAsync(string activationToken)
    {
        await _page.GotoAsync($"/activateaccount/{activationToken}");
    }
    
    /// <summary>
    /// Fill activation form and submit
    /// </summary>
    public async Task ActivateAccountAsync(string newPassword)
    {
        await NewPasswordField.FillAsync(newPassword);
        await ConfirmPasswordField.FillAsync(newPassword);
        await SubmitButton.ClickAsync();
    }
    
    /// <summary>
    /// Click the "Go to Login Screen" button after successful activation
    /// </summary>
    public async Task GoToLoginAsync()
    {
        await GoToLoginButton.ClickAsync();
    }
    
    /// <summary>
    /// Verify activation was successful
    /// </summary>
    public async Task<bool> IsActivationSuccessfulAsync()
    {
        try
        {
            // Wait for success message to appear (React needs time to render)
            await SuccessMessage.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    /// <summary>
    /// Get the pre-filled username value
    /// </summary>
    public async Task<string?> GetUsernameAsync()
    {
        return await UsernameField.InputValueAsync();
    }
}
