using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Xspire.E2E.Playwright.Infrastructure;

/// <summary>
/// Sắp xếp test case theo <see cref="TestPriorityAttribute"/>; không gắn attribute thì ưu tiên cuối.
/// </summary>
public sealed class PriorityOrderer : ITestCaseOrderer
{
    public const string TypeName = "Xspire.E2E.Playwright.Infrastructure.PriorityOrderer";
    public const string AssemblyName = "Xspire.E2E.Playwright";

    public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
        where TTestCase : ITestCase =>
        testCases.OrderBy(GetPriority);

    private static int GetPriority<TTestCase>(TTestCase testCase) where TTestCase : ITestCase
    {
        var aqn = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
        var attr = testCase.TestMethod.Method.GetCustomAttributes(aqn).FirstOrDefault();
        if (attr == null)
            return int.MaxValue;

        var args = attr.GetConstructorArguments()?.ToArray();
        if (args is { Length: > 0 } && args[0] is int p)
            return p;

        return int.MaxValue;
    }
}
