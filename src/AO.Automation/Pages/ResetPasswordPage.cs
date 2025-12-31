using Microsoft.Playwright;

namespace AO.Automation.Pages;

/// <summary>
/// Page Object Model for the Reset Password page
/// </summary>
public class ResetPasswordPage
{
    private readonly IPage _page;
    
    // Locators
    private ILocator NewPasswordField => _page.GetByLabel("New Password", new() { Exact = true });
    private ILocator ConfirmPasswordField => _page.GetByLabel("Confirm New Password");
    private ILocator SubmitButton => _page.GetByRole(AriaRole.Button, new() { Name = "Submit" });
    private ILocator SuccessMessage => _page.GetByRole(AriaRole.Heading, new() { Name = "Your password has been reset successfully." });
    private ILocator GoToLoginButton => _page.GetByRole(AriaRole.Button, new() { Name = "Log in" });
    
    public ResetPasswordPage(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Navigate to the reset password page with token
    /// </summary>
    public async Task NavigateAsync(string resetToken)
    {
        await _page.GotoAsync($"/resetpassword/{resetToken}");
    }
    
    /// <summary>
    /// Fill reset password form and submit
    /// </summary>
    public async Task ResetPasswordAsync(string newPassword)
    {
        await NewPasswordField.FillAsync(newPassword);
        await ConfirmPasswordField.FillAsync(newPassword);
        await SubmitButton.ClickAsync();
    }
    
    /// <summary>
    /// Click "Go to Login Screen" button after successful reset
    /// </summary>
    public async Task GoToLoginAsync()
    {
        await GoToLoginButton.ClickAsync();
    }
    
    /// <summary>
    /// Verify password reset was successful
    /// </summary>
    public async Task<bool> IsResetSuccessfulAsync()
    {
        try
        {
            await SuccessMessage.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
