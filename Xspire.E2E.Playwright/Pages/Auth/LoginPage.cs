using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.Auth;

public class LoginPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public LoginPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Region: locators (public get-only) so that tests can assert on them

    public ILocator ImageBrandLogo =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[1]/div[1]");

    public ILocator StringBrandName =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[1]/div[2]");

    public ILocator StringFormHeader =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[2]/div/div[1]/h2");

    public ILocator ComboLanguage =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[2]/div/div[1]/div/button");

    public ILocator StringTenant =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[2]/div/div[2]/div/div[1]/span");

    public ILocator StringTenantName =>
        _page.Locator("xpath=//html/body/div/div/div/div/div/div/div[2]/div/div[2]/div/div[1]/h6/span");

    public ILocator ButtonSwitchTenant =>
        _page.Locator("#AbpTenantSwitchLink");

    public ILocator UserNameInput =>
        _page.Locator("#LoginInput_UserNameOrEmailAddress");

    public ILocator PasswordInput =>
        _page.Locator("#password-input");

    public ILocator ButtonPasswordVisibility =>
        _page.Locator("#PasswordVisibilityButton");

    public ILocator CheckboxRememberMe =>
        _page.Locator("#LoginInput_RememberMe");

    public ILocator LinkForgotPassword =>
        _page.Locator("#loginForm div:nth-of-type(2) a");

    public ILocator LoginButton =>
        _page.Locator("#loginForm button[type=submit]");

    public ILocator StringErrorAlert =>
        _page.Locator("#AbpPageAlerts div");

    public ILocator ButtonCloseErrorAlert =>
        _page.Locator("#AbpPageAlerts div button");

    public ILocator StringErrorUsername =>
        _page.Locator("#LoginInput_UserNameOrEmailAddress-error");

    public ILocator StringErrorPassword =>
        _page.Locator("#password-input-error");

    // Region: navigation / high-level actions

    public Task GotoAsync() =>
        _page.GotoAsync("Account/Login");

    public async Task EnsureLoginPageAsync()
    {
        // Always navigate explicitly to the login page, then wait for DOM loaded.
        await GotoAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task LoginAsync(string username, string password)
    {
        await UserNameInput.FillAsync(username);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public Task<bool> IsErrorVisibleAsync() =>
        StringErrorAlert.IsVisibleAsync();

    // Region: helpers used by individual test cases

    public async Task FillSuccessAccountAsync()
    {
        await UserNameInput.FillAsync(_settings.ValidUser);
        await PasswordInput.FillAsync(_settings.ValidPassword);
    }

    public async Task FillFailedAccountAsync()
    {
        // Use obviously invalid credentials so that login must fail.
        var invalidUser = _settings.ValidUser + "_invalid";
        var invalidPassword = _settings.ValidPassword + "_invalid";

        await UserNameInput.FillAsync(invalidUser);
        await PasswordInput.FillAsync(invalidPassword);
    }

    public async Task FillEmptyUsernameAsync()
    {
        // Only fill password to trigger username required validation.
        await PasswordInput.FillAsync(_settings.ValidPassword);
    }

    public async Task FillEmptyPasswordAsync()
    {
        // Only fill username to trigger password required validation.
        await UserNameInput.FillAsync(_settings.ValidUser);
    }

    public async Task ClickCloseErrorAlertAsync()
    {
        await ButtonCloseErrorAlert.ClickAsync();
    }

    public async Task ClickForgotPasswordAsync()
    {
        await LinkForgotPassword.ClickAsync();
    }
}