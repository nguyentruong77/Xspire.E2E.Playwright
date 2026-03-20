using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.Inventory.UnitOfMeasures;

/// <summary>
/// Màn list Unit Of Measures dưới Inventory.
/// Cấu trúc tương tự CountriesPage (list + toolbar + grid).
/// </summary>
public class UnitOfMeasuresListPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public UnitOfMeasuresListPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // ===== Navigation & assertion helpers =====

    public async Task EnsureOnUnitOfMeasuresPageAsync()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*UnitOfMeasures.*"));
    }

    /// <summary>
    /// Điều hướng về list UnitOfMeasures bằng URL trực tiếp (tránh phụ thuộc menu click),
    /// sau đó chờ nút New hiện lên để xác nhận list đã sẵn sàng.
    /// </summary>
    public async Task EnsureOnListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/Inventory/UnitOfMeasures");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await EnsureOnUnitOfMeasuresPageAsync();
        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
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

    public async Task SearchByUnitNameAsync(string unitName)
    {
        await SearchInput.FillAsync(unitName);
        await SearchInput.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    // ===== Grid =====

    /// <summary>Tất cả link trong cột "Unit Name" của grid.</summary>
    public ILocator GridUnitNameCellLink =>
        _page.Locator("td[data-caption='Unit Name'] a");

    /// <summary>
    /// Assert rằng grid có ít nhất 1 dòng với Unit Name khớp chính xác giá trị mong đợi.
    /// </summary>
    public async Task EnsureUnitNameInGridAsync(string expectedName)
    {
        var cell = GridUnitNameCellLink.Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(expectedName)}$")
        });
        await Assertions.Expect(cell.First).ToBeVisibleAsync(new() { Timeout = _settings.StandardTimeoutMs });
    }
}
