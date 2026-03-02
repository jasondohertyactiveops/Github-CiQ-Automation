using System.Reflection;
using AO.Automation.Shared.Attributes;

namespace AO.Automation.API.Client.Tests.Conventions;

[Trait("Category", "Convention")]
public class RequireAzureTestCaseAttribute
{
    private readonly Type[] _testClasses;

    public RequireAzureTestCaseAttribute()
    {
        _testClasses = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t != GetType())
            .Where(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Any(m => m.GetCustomAttributes().Any(a =>
                    a.GetType().Name is "FactAttribute" or "TheoryAttribute")))
            .ToArray();
    }

    [Fact]
    public void AllTestClasses_MustHaveAzureTestSuiteAttribute()
    {
        var violators = _testClasses
            .Where(t => t.GetCustomAttribute<AzureTestSuiteAttribute>() == null)
            .Select(t => t.Name)
            .ToList();

        Assert.True(violators.Count == 0,
            $"The following test classes are missing [AzureTestSuite]:\n  - {string.Join("\n  - ", violators)}");
    }

    [Fact]
    public void AllTestClasses_MustHaveAzureTestCaseAttribute()
    {
        var violators = _testClasses
            .Where(t => t.GetCustomAttribute<AzureTestCaseAttribute>() == null)
            .Select(t => t.Name)
            .ToList();

        Assert.True(violators.Count == 0,
            $"The following test classes are missing [AzureTestCase]:\n  - {string.Join("\n  - ", violators)}");
    }

    [Fact]
    public void AllTestClasses_MustHaveAtLeastOneAzureTestPlanAttribute()
    {
        var violators = _testClasses
            .Where(t => !t.GetCustomAttributes<AzureTestPlanAttribute>().Any())
            .Select(t => t.Name)
            .ToList();

        Assert.True(violators.Count == 0,
            $"The following test classes are missing [AzureTestPlan]:\n  - {string.Join("\n  - ", violators)}");
    }

    [Fact]
    public void AllTestPlans_MustBeSmokeOrRegression()
    {
        var allowed = new[] { "Smoke", "Regression" };

        var violators = _testClasses
            .SelectMany(t => t.GetCustomAttributes<AzureTestPlanAttribute>()
                .Where(a => !allowed.Contains(a.Plan))
                .Select(a => $"{t.Name}: \"{a.Plan}\""))
            .ToList();

        Assert.True(violators.Count == 0,
            $"The following test classes have invalid [AzureTestPlan] values (must be Smoke or Regression):\n  - {string.Join("\n  - ", violators)}");
    }

    [Fact]
    public void AllTestMethods_MustHaveAzureTestStepAttribute()
    {
        var violators = _testClasses
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            .Where(m => m.GetCustomAttributes().Any(a =>
                a.GetType().Name is "FactAttribute" or "TheoryAttribute"))
            .Where(m => !m.GetCustomAttributes<AzureTestStepAttribute>().Any())
            .Select(m => $"{m.DeclaringType!.Name}.{m.Name}")
            .ToList();

        Assert.True(violators.Count == 0,
            $"The following test methods are missing [AzureTestStep]:\n  - {string.Join("\n  - ", violators)}");
    }
}
