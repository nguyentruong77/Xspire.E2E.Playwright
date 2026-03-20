using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.GeoSubdivisions.Countries;

/// <summary>
/// Màn form New Country (Countries - New) dưới Shared Information &gt; Geo Subdivisions.
/// Chỉ tập trung các field bắt buộc: Code, Time Zone, Description và toolbar Back/Save.
/// </summary>
public class CountriesNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public CountriesNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    /// <summary>
    /// Ensure đã vào đúng màn Countries New: URL chứa GUID toàn 0 (tương tự TaxCategoriesNew).
    /// </summary>
    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*SharedInformation/Countries/00000000-0000-0000-0000-000000000000.*"));
    }

    /// <summary>
    /// Ensure đang ở màn Edit Country:
    /// - URL phải là /SharedInformation/Countries/{GUID thật} (không còn all-zero GUID của màn New).
    /// - Ô Code trong form edit phải có value đúng mã expectedCode (ví dụ CPC).
    /// </summary>
    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/SharedInformation/Countries/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    // ===== Required fields =====

    /// <summary>Textbox Code (name = "ne_Txt_Code").</summary>
    public ILocator InputCode =>
        _page.Locator("input[name='ne_Txt_Code']");

    /// <summary>Textbox Description – chỉ bắt input HTML (name), tránh trùng wrapper dxbl-input-editor id.</summary>
    public ILocator InputDescription =>
        _page.Locator("input[name='ne_Txt_Description']");

    /// <summary>
    /// Combobox Time Zone: id trên dxbl-combo-box không còn cố định; neo theo form layout item thứ 2
    /// (cùng cấu trúc DOM: form/dxbl-form-layout/div/dxbl-form-layout-item[2]/div/dxbl-combo-box/...).
    /// </summary>
    private ILocator TimeZoneComboBox =>
        _page.Locator("xpath=//form/dxbl-form-layout/div/dxbl-form-layout-item[2]/div/dxbl-combo-box");

    public ILocator TimeZoneComboInput => TimeZoneComboBox.Locator("input");

    public ILocator TimeZoneDropdownButton =>
        TimeZoneComboBox.Locator(".dxbl-btn-group-right button");

    // ===== Toolbar buttons (Back / Save) =====

    /// <summary>
    /// Tập hợp các nút trên thanh toolbar của form (Back, Save, ...).
    /// Selector tạm thời: mọi button có class dxbl-btn trong form.
    /// </summary>
    public ILocator ToolbarButtons =>
        _page.Locator("button.dxbl-btn");

    /// <summary>Lấy button theo text hiển thị (ví dụ \"Save\", \"Back\").</summary>
    public ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    public async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public Task ClickSaveAsync() => ClickToolbarButtonAsync("Save");

    public Task ClickBackAsync() => ClickToolbarButtonAsync("Back");

    // ===== Helper methods cho testcase success =====

    /// <summary>
    /// Chọn Time Zone trong combobox: click nút dropdown, gõ filter, chọn option theo text.
    /// DevExpress list có thể dùng role="option" hoặc class .dxbl-list-box-item; thử cả hai và chờ list hiện.
    /// </summary>
    public async Task SelectTimeZoneAsync(string displayText)
    {
        // 1. Mở dropdown bằng nút bên phải combobox
        await TimeZoneDropdownButton.ClickAsync();

        // 2. Focus vào input rồi gõ filter (tránh gõ nhầm chỗ khác)
        await TimeZoneComboInput.ClickAsync();
        await TimeZoneComboInput.FillAsync(displayText);

        // 3. Chờ list xổ ra và có ít nhất một option (list có thể render trong popup)
        await _page.WaitForTimeoutAsync(500); // đợi filter/search cập nhật

        // 4. Chọn option: ưu tiên GetByRole(Option), fallback sang .dxbl-list-box-item
        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    /// <summary>
    /// Fill các field bắt buộc cho testcase success (Code, Time Zone, Description).
    /// Giá trị cụ thể sẽ được truyền từ test hoặc từ TestData riêng cho Countries.
    /// </summary>
    public async Task FillRequiredFieldsAsync(string code, string timeZoneText, string description)
    {
        await InputCode.FillAsync(code);
        await SelectTimeZoneAsync(timeZoneText);
        await InputDescription.FillAsync(description);
    }
}

