namespace AO.Automation.BaseClasses;

/// <summary>
/// Base class for all Playwright tests. Handles browser and page lifecycle.
/// All test classes should inherit from this.
/// </summary>
public abstract class PlaywrightTest : IAsyncLifetime
{
    protected TestConfig Config => TestConfig.Instance;
    protected IPlaywright? Playwright { get; private set; }
    protected IBrowser? Browser { get; private set; }
    protected IBrowserContext? Context { get; private set; }
    protected IPage Page { get; private set; } = null!;
    
    public virtual async Task InitializeAsync()
    {
        // Create Playwright instance
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        
        // Launch browser based on config
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Config.Headless
        });
        
        // Create browser context with default options
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = Config.BaseUrl,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true
        });
        
        // Create page
        Page = await Context.NewPageAsync();
    }

    public virtual async Task DisposeAsync()
    {
        // Cleanup
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.CloseAsync();
        if (Browser != null) await Browser.CloseAsync();
        Playwright?.Dispose();
    }
}
