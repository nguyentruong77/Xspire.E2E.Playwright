using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.Inventory.Warehouse;

/// <summary>
/// Màn list Warehouse (Danh Mục Kho) dưới Inventory.
/// Cấu trúc tương tự UnitOfMeasuresListPage (list + toolbar + grid).
/// </summary>
public class WarehouseListPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public WarehouseListPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // ===== Navigation & assertion helpers =====

    public async Task EnsureOnWarehousesPageAsync()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*Warehouses.*"));
    }

    /// <summary>
    /// Link nhóm sidebar: &lt;a href="/Inventory/Warehouses" class="group-button"&gt;…&lt;span class="group-text"&gt;Danh Mục Kho&lt;/span&gt;&lt;/a&gt;
    /// </summary>
    public ILocator LinkDanhMucKho =>
        _page.Locator("a.group-button[href='/Inventory/Warehouses']");

    /// <summary>
    /// Điều hướng về list Warehouse bằng URL trực tiếp (tránh phụ thuộc menu click),
    /// sau đó chờ nút New hiện lên để xác nhận list đã sẵn sàng.
    /// </summary>
    public async Task EnsureOnListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/Inventory/Warehouses");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await EnsureOnWarehousesPageAsync();
        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>Điều hướng qua link "Danh Mục Kho" trên sidebar (khi đã ở khu vực Inventory).</summary>
    public async Task NavigateViaDanhMucKhoLinkAsync()
    {
        await LinkDanhMucKho.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await LinkDanhMucKho.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await EnsureOnWarehousesPageAsync();
    }

    // ===== Toolbar buttons =====

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    public ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    public async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>Nút New (có thể icon-only); ưu tiên accessible name.</summary>
    public ILocator ButtonNew =>
        _page.GetByRole(AriaRole.Button, new() { Name = "New" });

    public async Task OpenNewFormAsync()
    {
        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await ButtonNew.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    // ===== Search =====

    public ILocator SearchInput =>
        _page.Locator("dxbl-input-editor input[type='text']").First;

    public async Task SearchByTextAsync(string text)
    {
        await SearchInput.FillAsync(text);
        await SearchInput.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
