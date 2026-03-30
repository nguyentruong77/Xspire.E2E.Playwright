using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Inventory.Warehouse;

namespace Xspire.E2E.Playwright.Tests.Inventory.Warehouse;

/// <summary>
/// Warehouse validation suite (TC-WH-001 ..).
/// 1 class = 1 browser (TestBase), các test case chạy nối tiếp.
/// </summary>
[Collection("E2ESuite")]
public class WarehouseTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public WarehouseTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    // ===== Shared helpers =====

    private async Task EnsureLoggedInAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var url = page.Url ?? string.Empty;
        var needLogin =
            string.IsNullOrWhiteSpace(url) ||
            url == "about:blank" ||
            url.Contains("Account/Login", System.StringComparison.OrdinalIgnoreCase);

        if (needLogin)
        {
            var loginPage = new LoginPage(page, settings);
            await loginPage.EnsureLoginPageAsync();
            await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    private async Task EnsureOnWarehouseListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var listPage = new WarehouseListPage(page, settings);

        if (!page.Url.Contains("/Inventory/Warehouses", System.StringComparison.OrdinalIgnoreCase))
        {
            await listPage.EnsureOnListAsync();
        }
        else
        {
            await listPage.EnsureOnWarehousesPageAsync();
            await listPage.ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = settings.StandardTimeoutMs
            });
        }
    }

    // ===== Test cases =====

    [Fact]
    public async Task TC_WH_001_Open_New_Warehouse_Form()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var listPage = new WarehouseListPage(page, settings);
        var newPage = new WarehouseNewPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnWarehouseListAsync();

        // Thao tác đầu tiên: nhấn button New
        // HTML: <button class="dxbl-btn dxbl-btn-primary dxbl-btn-icon-only dxbl-btn-standalone" id="id72c0f5a9-29cc-43ef-a2cb-223207ebd9ea" type="button">New</button>
        await listPage.OpenNewFormAsync();

        // Kiểm tra đã chuyển sang trang tạo mới (GUID toàn 0)
        await newPage.EnsureOnNewPageAsync();

        // Nhập 123 vào Code và Description
        // HTML: <input name="ne_Txt_Code" ...>
        // HTML: <input name="ne_Txt_Description" ...>
        await newPage.FillCodeAndDescriptionAsync("123", "123");

        // Nhấn checkbox Active
        // HTML: <input type="checkbox" name="ne_Chk_Active" ...>
        await newPage.CheckboxActive.First.ClickAsync();

        // Nhấn checkbox Is Public
        // HTML: <input type="checkbox" name="ne_Chk_IsPublic" ...>
        await newPage.CheckboxIsPublic.First.ClickAsync();
    }
}
