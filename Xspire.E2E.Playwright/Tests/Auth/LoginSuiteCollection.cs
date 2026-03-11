using Xunit;

namespace Xspire.E2E.Playwright.Tests.Auth;

/// <summary>
/// Login tests share one browser/page and must run sequentially to avoid session conflicts.
/// </summary>
[CollectionDefinition("LoginSuite", DisableParallelization = true)]
public class LoginSuiteCollection { }
