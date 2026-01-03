using AO.Automation.UI.Client.Config;
using Microsoft.Playwright;

namespace AO.Automation.UI.Client.BaseClasses;

/// <summary>
/// Base class for all Playwright tests. Handles page lifecycle.
/// Browser is shared via BrowserFixture (one per test class).
/// Each test gets its own Context and Page (isolated).
/// Test classes should implement IClassFixture<BrowserFixture>.
/// </summary>
public abstract class PlaywrightTest : IAsyncLifetime
{
    protected TestConfig Config => TestConfig.Instance;
    protected IBrowser Browser { get; private set; } = null!;
    protected IBrowserContext Context { get; private set; } = null!;
    protected IPage Page { get; private set; } = null!;
    
    private readonly BrowserFixture _browserFixture;
    
    protected PlaywrightTest(BrowserFixture browserFixture)
    {
        _browserFixture = browserFixture;
    }
    
    public virtual async Task InitializeAsync()
    {
        // Get shared browser from fixture
        Browser = _browserFixture.Browser;
        
        // Create NEW context and page for each test (isolated)
        Context = await Browser.NewContextAsync(new BrowserNewContextOptions
        {
            BaseURL = Config.BaseUrl,
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true
        });
        
        Page = await Context.NewPageAsync();
    }

    public virtual async Task DisposeAsync()
    {
        // Cleanup context and page after each test
        if (Page != null) await Page.CloseAsync();
        if (Context != null) await Context.CloseAsync();
        // Browser cleanup handled by BrowserFixture (shared resource)
    }
}
