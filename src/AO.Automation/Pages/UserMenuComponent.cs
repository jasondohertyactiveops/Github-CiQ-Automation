using Microsoft.Playwright;

namespace AO.Automation.Pages;

/// <summary>
/// Component for the User Menu (icon and dropdown navigation)
/// </summary>
public class UserMenuComponent
{
    private readonly IPage _page;
    
    private ILocator UserIcon => _page.GetByRole(AriaRole.Button, new() { Name = "User menu" });
    private ILocator UserInitials => _page.Locator("[data-tag='user-initials']");
    private ILocator Dropdown => _page.GetByRole(AriaRole.Tooltip).Or(_page.Locator("[role='tooltip']"));
    private ILocator MyAccountButton => _page.GetByRole(AriaRole.Button, new() { Name = "My Account" }).Or(_page.Locator("[data-tag='my-account-btn']"));
    private ILocator LogoutButton => _page.GetByRole(AriaRole.Button, new() { Name = "Logout" }).Or(_page.Locator("[data-tag='logout-btn']"));
    
    public UserMenuComponent(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Click user icon to open dropdown
    /// </summary>
    public async Task OpenMenuAsync()
    {
        // Close any notification dialogs that might be blocking
        await CloseNotificationIfPresentAsync();
        
        await UserIcon.ClickAsync();
    }
    
    /// <summary>
    /// Close blocking dialogs if present (RTM activity dialog, notifications)
    /// </summary>
    private async Task CloseNotificationIfPresentAsync()
    {
        try
        {
            // Wait for RTM "Select Your Activity" dialog to appear
            var rtmDialog = _page.GetByRole(AriaRole.Dialog, new() { Name = "Select Your Activity" });
            await rtmDialog.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 2000 });
            
            // Dialog is present, close it
            var closeButton = _page.GetByTestId("close-btn");
            await closeButton.ClickAsync();
            
            // Wait for dialog to close
            await rtmDialog.WaitForAsync(new() { State = WaitForSelectorState.Hidden, Timeout = 2000 });
        }
        catch
        {
            // Dialog not present or already closed, continue
        }
    }
    
    /// <summary>
    /// Get user initials displayed in icon
    /// </summary>
    public async Task<string?> GetInitialsAsync()
    {
        return await UserInitials.TextContentAsync();
    }
    
    /// <summary>
    /// Navigate to My Account page
    /// </summary>
    public async Task NavigateToMyAccountAsync()
    {
        await OpenMenuAsync();
        await MyAccountButton.ClickAsync();
    }
    
    /// <summary>
    /// Get full name from dropdown
    /// </summary>
    public async Task<string?> GetFullNameFromDropdownAsync()
    {
        return await Dropdown.Locator("[data-tag='user-name']").TextContentAsync();
    }
    
    /// <summary>
    /// Get email from dropdown
    /// </summary>
    public async Task<string?> GetEmailFromDropdownAsync()
    {
        return await Dropdown.Locator("[data-tag='email']").TextContentAsync();
    }
}
