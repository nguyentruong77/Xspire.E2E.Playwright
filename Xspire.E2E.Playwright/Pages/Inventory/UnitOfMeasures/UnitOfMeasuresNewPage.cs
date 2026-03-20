using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.Inventory.UnitOfMeasures;

namespace Xspire.E2E.Playwright.Pages.Inventory.UnitOfMeasures;

/// <summary>
/// Màn form New Unit Of Measure (URL chứa GUID toàn 0).
/// Cấu trúc tương tự TaxCategoryNewPage / CountriesNewPage.
/// </summary>
public class UnitOfMeasuresNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public UnitOfMeasuresNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // ===== Navigation assertions =====

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*UnitOfMeasures/00000000-0000-0000-0000-000000000000.*"));
    }

    /// <summary>
    /// Assert lưu thành công: URL chuyển sang GUID thật (không còn all-zero).
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*/Inventory/UnitOfMeasures/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    // ===== Form fields =====

    public ILocator InputFromUnit => _page.Locator("input[name='ne_Txt_FromUnit']");
    public ILocator InputToUnit => _page.Locator("input[name='ne_Txt_ToUnit']");

    // ===== Validation errors =====
    // DevExpress Blazor: không có #ne_Txt_*-error; lỗi hiển thị qua .dxbl-edit-validation-status
    // và/hoặc div.validation-message trong div.dxbl-fl-ctrl bọc dxbl-input-editor.

    private ILocator FromUnitFieldWrapper =>
        _page.Locator("div.dxbl-fl-ctrl").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("#ne_Txt_FromUnit")
        });

    private ILocator ToUnitFieldWrapper =>
        _page.Locator("div.dxbl-fl-ctrl").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("#ne_Txt_ToUnit")
        });

    /// <summary>Chỉ dùng div.validation-message (một phần tử); tránh Or với .dxbl-edit-validation-status gây strict mode (2 match).</summary>
    public ILocator StringErrorFromUnit =>
        FromUnitFieldWrapper.Locator("div.validation-message");

    public ILocator StringErrorToUnit =>
        ToUnitFieldWrapper.Locator("div.validation-message");

    /// <summary>Icon lỗi duplicate (dxbl-edit-validation-status bên trong wrapper FromUnit).</summary>
    public ILocator FromUnitDuplicateError =>
        _page.Locator("#ne_Txt_FromUnit .dxbl-edit-validation-status");

    // ===== Toolbar =====

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    public ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    public async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public Task ClickSaveAsync() => ClickToolbarButtonAsync("Save");
    public Task ClickBackAsync() => ClickToolbarButtonAsync("Back");

    // ===== Support methods (tương tự TaxCategoryNewPage) =====

    public async Task FillUnitsAsync(string fromUnit, string toUnit)
    {
        await InputFromUnit.FillAsync(fromUnit);
        await InputToUnit.FillAsync(toUnit);
    }

    public async Task FillFromUnitOnlyAsync(string fromUnit)
    {
        await InputFromUnit.FillAsync(fromUnit);
    }

    public async Task FillToUnitOnlyAsync(string toUnit)
    {
        await InputToUnit.FillAsync(toUnit);
    }

    public async Task FillValidDataAsync()
    {
        await InputFromUnit.FillAsync(UnitOfMeasuresTestData.CreateSuccess.FromUnit);
        await InputToUnit.FillAsync(UnitOfMeasuresTestData.CreateSuccess.ToUnit);
    }
}
