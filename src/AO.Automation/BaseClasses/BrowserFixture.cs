namespace AO.Automation.BaseClasses;

/// <summary>
/// Browser fixture for xUnit - creates browser once per test class.
/// Shared across all tests in a class to improve performance.
/// </summary>
public class BrowserFixture : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    
    public async Task InitializeAsync()
    {
        // Create Playwright instance once
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        // Launch browser once based on config
        var config = TestConfig.Instance;
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = config.Headless
        });
    }

    public async Task DisposeAsync()
    {
        // Cleanup - runs once after all tests in the class complete
        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }
}
