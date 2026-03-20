namespace Xspire.E2E.Playwright.Infrastructure;

/// <summary>
/// Gắn trên method test để <see cref="PriorityOrderer"/> sắp xếp thứ tự chạy (số nhỏ chạy trước).
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TestPriorityAttribute : Attribute
{
    public TestPriorityAttribute(int priority) => Priority = priority;

    public int Priority { get; }
}
