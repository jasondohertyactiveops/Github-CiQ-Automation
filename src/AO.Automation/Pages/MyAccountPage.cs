using Microsoft.Playwright;

namespace AO.Automation.Pages;

/// <summary>
/// Page Object Model for the My Account page
/// </summary>
public class MyAccountPage
{
    private readonly IPage _page;
    
    // Page title
    private ILocator PageTitle => _page.Locator("main h4");
    
    // Employee Details Card
    private ILocator EmployeeDetailsCard => _page.Locator("[data-tag='employeedetails-card']");
    private ILocator UserTypeValue => EmployeeDetailsCard.Locator("[data-tag='user-type']");
    private ILocator EmploymentTypeValue => EmployeeDetailsCard.Locator("[data-tag='employement-type']");
    private ILocator RefIdValue => EmployeeDetailsCard.Locator("[data-tag='ref-id']");
    private ILocator LocationValue => EmployeeDetailsCard.Locator("[data-tag='location']");
    
    // Login Details Card
    private ILocator LoginDetailsCard => _page.Locator("[data-tag='logindetails-card']");
    private ILocator UsernameValue => LoginDetailsCard.Locator("[data-tag='username']");
    private ILocator ChangePasswordButton => LoginDetailsCard.Locator("[data-tag='change-password-btn']");
    
    public MyAccountPage(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Get page title text (should be user's full name)
    /// </summary>
    public async Task<string?> GetPageTitleAsync()
    {
        return await PageTitle.TextContentAsync();
    }
    
    /// <summary>
    /// Get User Type from Employee Details
    /// </summary>
    public async Task<string?> GetUserTypeAsync()
    {
        return await UserTypeValue.TextContentAsync();
    }
    
    /// <summary>
    /// Get Employment Type from Employee Details
    /// </summary>
    public async Task<string?> GetEmploymentTypeAsync()
    {
        return await EmploymentTypeValue.TextContentAsync();
    }
    
    /// <summary>
    /// Get Reference ID from Employee Details
    /// </summary>
    public async Task<string?> GetRefIdAsync()
    {
        return await RefIdValue.TextContentAsync();
    }
    
    /// <summary>
    /// Get Location from Employee Details
    /// </summary>
    public async Task<string?> GetLocationAsync()
    {
        return await LocationValue.TextContentAsync();
    }
    
    /// <summary>
    /// Get Username from Login Details
    /// </summary>
    public async Task<string?> GetUsernameAsync()
    {
        return await UsernameValue.TextContentAsync();
    }
    
    /// <summary>
    /// Verify Change Password button is enabled
    /// </summary>
    public async Task<bool> IsChangePasswordButtonEnabledAsync()
    {
        var isActive = await ChangePasswordButton.GetAttributeAsync("data-active");
        return isActive == "true";
    }
    
    /// <summary>
    /// Click Change Password button to open dialog
    /// </summary>
    public async Task ClickChangePasswordButtonAsync()
    {
        await ChangePasswordButton.ClickAsync();
    }
}
