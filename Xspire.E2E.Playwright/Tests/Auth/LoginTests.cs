using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;

namespace Xspire.E2E.Playwright.Tests.Auth;

/// <summary>
/// Login Validation Suite - maps to TS LoginValidationSuite.spec.ts (TC-LOGIN-001 .. TC-LOGIN-008).
/// 1 class = 1 browser (TestBase), các test case trong class chạy nối tiếp.
/// </summary>
[Collection("E2ESuite")]
public class LoginTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public LoginTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task TC_LOGIN_001_Validate_Login_Page_Visibility()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var loginPage = new LoginPage(page, settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // App may show "Xspire" or "Login | Xspire" depending on locale
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex(".*Xspire.*"));
        await Assertions.Expect(loginPage.ImageBrandLogo).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringBrandName).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringBrandName).ToHaveTextAsync("Xspire");
        await Assertions.Expect(loginPage.StringFormHeader).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringFormHeader).ToHaveTextAsync(new Regex("Login|Đăng nhập"));
        await Assertions.Expect(loginPage.ComboLanguage).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.ComboLanguage).ToContainTextAsync(new Regex("English|Tiếng Việt"));
        await Assertions.Expect(loginPage.StringTenant).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringTenant).ToHaveTextAsync(new Regex("Tenant|Người thuê"));
        await Assertions.Expect(loginPage.StringTenantName).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringTenantName).ToHaveTextAsync(new Regex("Not selected|Chưa chọn|Không được chọn"));
        await Assertions.Expect(loginPage.ButtonSwitchTenant).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.ButtonSwitchTenant).ToHaveTextAsync(new Regex("switch"));
        await Assertions.Expect(loginPage.UserNameInput).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.PasswordInput).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.ButtonPasswordVisibility).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.CheckboxRememberMe).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.CheckboxRememberMe).Not.ToBeCheckedAsync();
        await Assertions.Expect(loginPage.LinkForgotPassword).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.LinkForgotPassword).ToHaveTextAsync(new Regex("Forgot password\\?|Quên mật khẩu"));
        await Assertions.Expect(loginPage.LoginButton).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.LoginButton).ToContainTextAsync(new Regex("Login|Đăng nhập"));
        await Assertions.Expect(loginPage.StringErrorAlert).ToBeHiddenAsync();
    }

    [Fact]
    public async Task TC_LOGIN_002_Should_Show_Validation_Errors_For_Empty_Form()
    {
        var page = _fixture.Page;
        var loginPage = new LoginPage(page, _fixture.Settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await loginPage.LoginButton.ClickAsync();

        await Assertions.Expect(loginPage.StringErrorUsername).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringErrorUsername).ToHaveTextAsync(new Regex("The User name or email address field is required\\.|Trường User name or email address là bắt buộc\\."));
        await Assertions.Expect(loginPage.StringErrorPassword).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringErrorPassword).ToHaveTextAsync(new Regex("The Password field is required\\.|Trường Password là bắt buộc\\."));
    }

    [Fact]
    public async Task TC_LOGIN_003_Should_Show_Validation_Error_For_Empty_Username()
    {
        var page = _fixture.Page;
        var loginPage = new LoginPage(page, _fixture.Settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await loginPage.FillEmptyUsernameAsync();
        await loginPage.LoginButton.ClickAsync();

        await Assertions.Expect(loginPage.StringErrorUsername).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringErrorUsername).ToHaveTextAsync(new Regex("The User name or email address field is required\\.|Trường User name or email address là bắt buộc\\."));
        await Assertions.Expect(loginPage.StringErrorPassword).ToBeHiddenAsync();
    }

    [Fact]
    public async Task TC_LOGIN_004_Should_Show_Validation_Error_For_Empty_Password()
    {
        var page = _fixture.Page;
        var loginPage = new LoginPage(page, _fixture.Settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await loginPage.FillEmptyPasswordAsync();
        await loginPage.LoginButton.ClickAsync();

        await Assertions.Expect(loginPage.StringErrorPassword).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.StringErrorPassword).ToHaveTextAsync(new Regex("The Password field is required\\.|Trường Password là bắt buộc\\."));
        await Assertions.Expect(loginPage.StringErrorUsername).ToBeHiddenAsync();
    }

    [Fact]
    public async Task TC_LOGIN_005_Should_Show_Error_For_Invalid_Credentials()
    {
        var page = _fixture.Page;
        var loginPage = new LoginPage(page, _fixture.Settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await loginPage.FillFailedAccountAsync();
        await loginPage.LoginButton.ClickAsync();

        await Assertions.Expect(loginPage.StringErrorAlert).ToBeVisibleAsync();
        // App may show English or Vietnamese message
        await Assertions.Expect(loginPage.StringErrorAlert).ToContainTextAsync(new Regex("Invalid username or password!|Sai mật khẩu"));

        await loginPage.ClickCloseErrorAlertAsync();
        await Assertions.Expect(loginPage.StringErrorAlert).ToBeHiddenAsync();
    }

    [Fact]
    public async Task TC_LOGIN_006_Should_Navigate_To_Forgot_Password_Page()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var loginPage = new LoginPage(page, settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await loginPage.ClickForgotPasswordAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(page).ToHaveURLAsync(new Regex(".*Account/ForgotPassword.*"));
        await Assertions.Expect(page).ToHaveTitleAsync(new Regex("Forgot password\\? \\| Xspire|Quên mật khẩu.*Xspire"));
    }

    [Fact]
    public async Task TC_LOGIN_007_Should_Handle_Password_Field_As_Sensitive_Input()
    {
        var page = _fixture.Page;
        var loginPage = new LoginPage(page, _fixture.Settings);

        await loginPage.EnsureLoginPageAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(loginPage.PasswordInput).ToBeVisibleAsync();
        await Assertions.Expect(loginPage.PasswordInput).ToHaveAttributeAsync("type", "password");
    }

    [Fact]
    public async Task TC_LOGIN_008_Should_Login_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var loginPage = new LoginPage(page, settings);
        var homePage = new HomePage(page, settings);

        await loginPage.EnsureLoginPageAsync();
        await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(page).ToHaveTitleAsync("Xspire");

        await homePage.LogoutAsync();
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(page).ToHaveTitleAsync("Xspire");
        await Assertions.Expect(page).ToHaveURLAsync(settings.BaseUrl.TrimEnd('/') + "/");
    }
}
