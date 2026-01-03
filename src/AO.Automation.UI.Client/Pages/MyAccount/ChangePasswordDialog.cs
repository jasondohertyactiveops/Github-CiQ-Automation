using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Pages.MyAccount;

/// <summary>
/// Component for Change Password dialog on My Account page
/// </summary>
public class ChangePasswordDialog
{
    private readonly IPage _page;
    
    private ILocator Dialog => _page.GetByRole(AriaRole.Dialog);
    private ILocator CurrentPasswordField => Dialog.GetByLabel("Current Password");
    private ILocator NewPasswordField => Dialog.GetByLabel("New Password", new() { Exact = true });
    private ILocator ConfirmPasswordField => Dialog.GetByLabel("Confirm New Password");
    private ILocator SubmitButton => Dialog.GetByRole(AriaRole.Button, new() { Name = "Save" });
    private ILocator SuccessMessage => _page.GetByText("Password Successfully Changed");
    
    public ChangePasswordDialog(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Fill password change form and submit
    /// </summary>
    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        await CurrentPasswordField.FillAsync(currentPassword);
        await NewPasswordField.FillAsync(newPassword);
        await ConfirmPasswordField.FillAsync(newPassword);
        await SubmitButton.ClickAsync();
    }
    
    /// <summary>
    /// Verify password change was successful
    /// </summary>
    public async Task<bool> IsChangeSuccessfulAsync()
    {
        try
        {
            await SuccessMessage.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
            return true;
        }
        catch
        {
            return false;
        }
    }
}
