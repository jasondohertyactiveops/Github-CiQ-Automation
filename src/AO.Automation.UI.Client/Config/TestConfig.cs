using AO.Automation.Shared.Config;

namespace AO.Automation.UI.Client.Config;

/// <summary>
/// UI test configuration (inherits all settings from shared base)
/// This class maintains the existing namespace for backwards compatibility
/// </summary>
public class TestConfig : TestConfigBase
{
    private static TestConfig? _instance;
    
    public static TestConfig Instance => _instance ??= new TestConfig();
    
    private TestConfig() : base()
    {
    }
}
