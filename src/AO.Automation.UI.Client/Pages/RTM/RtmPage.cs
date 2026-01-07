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
            // Locate the close button within the dialog (more specific than just GetByTestId)
            var closeButton = SelectActivityDialog.GetByTestId("close-btn");
            
            // Wait for close button to be visible AND clickable (Playwright auto-waits for actionability)
            await closeButton.WaitForAsync(new() 
            { 
                State = WaitForSelectorState.Visible, 
                Timeout = 10000 
            });
            
            // Click the close button - Playwright waits for it to be actionable
            await closeButton.ClickAsync(new() { Timeout = 10000 });
            
            // Wait for dialog to completely disappear from DOM
            await SelectActivityDialog.WaitForAsync(new() 
            { 
                State = WaitForSelectorState.Detached, 
                Timeout = 10000 
            });
        }
        catch (TimeoutException)
        {
            // Dialog not present or already closed - continue
        }
    }
}
