using Xunit;

namespace Xspire.E2E.Playwright.Tests.Auth;

/// <summary>
/// E2E suite: mỗi test class = 1 browser (IClassFixture&lt;TestBase&gt;), các [Fact] trong class chạy nối tiếp.
/// DisableParallelization = true → các class trong suite chạy tuần tự (chỉ 1 browser hiển thị tại một thời điểm).
/// </summary>
[CollectionDefinition("E2ESuite", DisableParallelization = true)]
public class E2ESuiteCollection { }
