using Microsoft.Playwright;

namespace AO.Automation.UI.Client.Pages.RTM;

/// <summary>
/// Page object for Real Time Management (RTM) page
/// </summary>
public class RtmPage
{
    private readonly IPage _page;
    
    private ILocator SelectActivityDialog => _page.GetByRole(AriaRole.Dialog, new() { Name = "Select Your Activity" });
    
    public RtmPage(IPage page)
    {
        _page = page;
    }
    
    /// <summary>
    /// Close the "Select Your Activity" dialog if present
    /// Most tests don't need this dialog, so call this after login to dismiss it
    /// RTM-specific tests should NOT call this if they need to interact with the dialog
    /// </summary>
    public async Task CloseSelectActivityDialogIfPresentAsync()
    {
        try
        {
            // Check if dialog is visible (short timeout)
            await SelectActivityDialog.WaitForAsync(new() 
            { 
                State = WaitForSelectorState.Visible, 
                Timeout = 2000 
            });
            
            // Dialog is present - press Escape to close
            await _page.Keyboard.PressAsync("Escape");
            
            // Wait for dialog to disappear
            await SelectActivityDialog.WaitForAsync(new() 
            { 
                State = WaitForSelectorState.Detached, 
                Timeout = 5000 
            });
        }
        catch (TimeoutException)
        {
            // Dialog not present or already closed - continue
        }
    }
}
