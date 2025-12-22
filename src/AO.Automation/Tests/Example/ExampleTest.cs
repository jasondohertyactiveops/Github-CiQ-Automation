using System.Text.RegularExpressions;

namespace AO.Automation.Tests.Example;

/// <summary>
/// Example test to verify project setup works correctly.
/// DELETE THIS FILE once real tests are created.
/// </summary>
[Trait("Suite", "Smoke")]
[Trait("Feature", "Example")]
public class ExampleTest : PlaywrightTest
{
    [Fact]
    public async Task CanNavigateToApplication()
    {
        // Arrange - Page is already available from base class
        
        // Act - Navigate to the base URL from config
        await Page.GotoAsync(Config.BaseUrl);
        
        // Assert - Page loaded successfully
        await Expect(Page).ToHaveTitleAsync(new Regex(".*"));
        
        // This test just proves:
        // 1. Project compiles
        // 2. Config loads correctly
        // 3. Playwright can navigate to your app
        // 4. Base test class works
    }
}
