using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xunit;

namespace Xspire.E2E.Playwright.Infrastructure;

public class TestBase : IAsyncLifetime
{
    public IPlaywright Playwright { get; private set; } = null!;
    public IBrowser Browser { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;
    public PlaywrightSettings Settings { get; private set; } = null!;

    public async Task InitializeAsync()
{
    Settings = PlaywrightSettings.Load();

    Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
    Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
    {
        Headless = Settings.Headless
    });

    var context = await Browser.NewContextAsync(new BrowserNewContextOptions
    {
        BaseURL = Settings.BaseUrl
    });

    Page = await context.NewPageAsync();

    // Dùng timeout từ config (mapping với ACTION/NAVIGATION)
    Page.SetDefaultTimeout(Settings.ActionTimeoutMs);
    Page.SetDefaultNavigationTimeout(Settings.NavigationTimeoutMs);
}

    public async Task DisposeAsync()
    {
        if (Page?.Context != null)
        {
            await Page.Context.CloseAsync();
        }

        if (Browser != null)
        {
            await Browser.CloseAsync();
        }

        Playwright?.Dispose();
    }
}