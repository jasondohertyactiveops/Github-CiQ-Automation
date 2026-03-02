using Xunit.Abstractions;
using Xunit.Sdk;

namespace AO.Automation.Shared.Attributes;

/// <summary>
/// Links a test class to an Azure DevOps test case.
/// Class-level only — use [AzureTestStep] on methods.
/// 
/// Usage:
///   [AzureTestCase(25057)]
/// </summary>
[TraitDiscoverer(AzureTestCaseDiscoverer.FullyQualifiedName, AzureTestCaseDiscoverer.AssemblyName)]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AzureTestCaseAttribute : Attribute, ITraitAttribute
{
    public int TestCaseId { get; }

    public AzureTestCaseAttribute(int testCaseId)
    {
        TestCaseId = testCaseId;
    }
}

/// <summary>
/// Links a test method to a step within an Azure DevOps test case.
/// Method-level only — step is required. Apply multiple times for multiple steps.
/// 
/// Usage:
///   [AzureTestStep(25057, 4)]                    → single step
///   [AzureTestStep(25058, 3)] [AzureTestStep(25058, 4)]  → multiple steps
/// </summary>
[TraitDiscoverer(AzureTestStepDiscoverer.FullyQualifiedName, AzureTestStepDiscoverer.AssemblyName)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AzureTestStepAttribute : Attribute, ITraitAttribute
{
    public int TestCaseId { get; }
    public int Step { get; }

    public AzureTestStepAttribute(int testCaseId, int step)
    {
        TestCaseId = testCaseId;
        Step = step;
    }

    /// <summary>
    /// Short label for logs/display: "TC25058:Step4"
    /// </summary>
    public string Label => $"TC{TestCaseId}:Step{Step}";
}

/// <summary>
/// Links a test class to an Azure DevOps test plan (e.g. Smoke, Regression).
/// Class-level only. Apply multiple times if the test case belongs to multiple plans.
/// 
/// Usage:
///   [AzureTestPlan("Smoke")]
///   [AzureTestPlan("Regression")]
/// </summary>
[TraitDiscoverer(AzureTestPlanDiscoverer.FullyQualifiedName, AzureTestPlanDiscoverer.AssemblyName)]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class AzureTestPlanAttribute : Attribute, ITraitAttribute
{
    public string Plan { get; }

    public AzureTestPlanAttribute(string plan)
    {
        Plan = plan;
    }
}

/// <summary>
/// Links a test class to an Azure DevOps test suite by ID.
/// Class-level only.
/// 
/// Usage:
///   [AzureTestSuite(25146)] // Login
/// </summary>
[TraitDiscoverer(AzureTestSuiteDiscoverer.FullyQualifiedName, AzureTestSuiteDiscoverer.AssemblyName)]
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class AzureTestSuiteAttribute : Attribute, ITraitAttribute
{
    public int SuiteId { get; }

    public AzureTestSuiteAttribute(int suiteId)
    {
        SuiteId = suiteId;
    }
}

public class AzureTestCaseDiscoverer : ITraitDiscoverer
{
    internal const string FullyQualifiedName = "AO.Automation.Shared.Attributes.AzureTestCaseDiscoverer";
    internal const string AssemblyName = "AO.Automation.Shared";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
        var tcId = ctorArgs.Count > 0 && ctorArgs[0] is int id ? id : 0;

        yield return new KeyValuePair<string, string>("TC", tcId.ToString());
    }
}

public class AzureTestPlanDiscoverer : ITraitDiscoverer
{
    internal const string FullyQualifiedName = "AO.Automation.Shared.Attributes.AzureTestPlanDiscoverer";
    internal const string AssemblyName = "AO.Automation.Shared";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
        var plan = ctorArgs.Count > 0 && ctorArgs[0] is string p ? p : string.Empty;

        yield return new KeyValuePair<string, string>("Plan", plan);
    }
}

public class AzureTestSuiteDiscoverer : ITraitDiscoverer
{
    internal const string FullyQualifiedName = "AO.Automation.Shared.Attributes.AzureTestSuiteDiscoverer";
    internal const string AssemblyName = "AO.Automation.Shared";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
        var suiteId = ctorArgs.Count > 0 && ctorArgs[0] is int id ? id : 0;

        yield return new KeyValuePair<string, string>("Suite", suiteId.ToString());
    }
}

public class AzureTestStepDiscoverer : ITraitDiscoverer
{
    internal const string FullyQualifiedName = "AO.Automation.Shared.Attributes.AzureTestStepDiscoverer";
    internal const string AssemblyName = "AO.Automation.Shared";

    public IEnumerable<KeyValuePair<string, string>> GetTraits(IAttributeInfo traitAttribute)
    {
        var ctorArgs = traitAttribute.GetConstructorArguments().ToList();
        var tcId = ctorArgs.Count > 0 && ctorArgs[0] is int id ? id : 0;

        yield return new KeyValuePair<string, string>("TC", tcId.ToString());

        if (ctorArgs.Count > 1 && ctorArgs[1] is int step)
        {
            yield return new KeyValuePair<string, string>("Step", step.ToString());
        }
    }
}
