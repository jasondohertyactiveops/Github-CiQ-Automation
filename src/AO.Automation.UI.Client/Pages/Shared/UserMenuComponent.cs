using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Pages.Shared;

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
        await UserIcon.ClickAsync();
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
