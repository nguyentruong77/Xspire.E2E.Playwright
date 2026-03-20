using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.Common;

public class HomePage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public HomePage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Locator avatar user ở góc trên (mở menu user)
    private ILocator UserAvatar => _page.Locator("#userItem a div img");

    // Locator menu Logout trong menu user
    private ILocator LogoutMenuItem => _page.Locator("#MenuItem_Account_Logout a span:nth-of-type(2)");

    // Menu item "Shared Information" trên sidebar trái (có thể có 2 bản: desktop + mobile → chọn cái đầu)
    private ILocator SharedInformationMenuItem => _page.Locator("#MenuItem_SI").First;

    // Menu item "Inventory" trên sidebar trái
    private ILocator InventoryMenuItem => _page.Locator("#MenuItem_IN").First;

    // Hành vi logout: click avatar -> click Logout
    public async Task LogoutAsync()
    {
        await UserAvatar.ClickAsync();
        await LogoutMenuItem.ClickAsync();
    }

    // Điều hướng sang trang Shared Information qua menu trái
    public async Task NavigateToSharedInformationAsync()
    {
        await SharedInformationMenuItem.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    // Điều hướng sang trang Inventory qua menu trái
    public async Task NavigateToInventoryAsync()
    {
        await InventoryMenuItem.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}