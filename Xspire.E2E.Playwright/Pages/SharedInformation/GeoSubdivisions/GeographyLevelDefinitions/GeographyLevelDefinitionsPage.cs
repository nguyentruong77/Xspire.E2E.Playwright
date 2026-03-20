using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.GeoSubdivisions.GeographyLevelDefinitions;

/// <summary>
/// Represents the Geography Level Definitions screen (list/home) under Shared Information &gt; Geo Subdivisions.
/// </summary>
public class GeographyLevelDefinitionsPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public GeographyLevelDefinitionsPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnGeographyLevelDefinitionsListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/SharedInformation/GeographyLevelDefinitions");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*SharedInformation/GeographyLevelDefinitions.*"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>
    /// Ensure lưu thành công dựa trên URL detail:
    /// - không còn GUID all-zero: .../GeographyLevelDefinitions/00000000-...
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/SharedInformation/GeographyLevelDefinitions/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    /// <summary>
    /// Từ màn detail (sau Save), bấm Back để về list.
    /// </summary>
    public async Task NavigateBackToListAsync()
    {
        await ClickToolbarButtonAsync("Back");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    // ===== Toolbar buttons (New/Back) =====

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    private async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public ILocator ButtonNew => GetToolbarButton("New");

    // ===== List actions =====

    public async Task FillSearchAsync(string code)
    {
        // Reuse existing approach: first textbox role in toolbar.
        var textbox = _page.GetByRole(AriaRole.Textbox).First;
        await textbox.ClickAsync();
        await textbox.FillAsync(code);
        await textbox.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Ensure search thành công: ô search có value đúng, và grid có cell Code khớp exact.
    /// </summary>
    public async Task EnsureSearchSuccessAsync(string expectedCode)
    {
        var timeout = _settings.StandardTimeoutMs;
        var textbox = _page.GetByRole(AriaRole.Textbox).First;
        await Assertions.Expect(textbox).ToHaveValueAsync(expectedCode, new() { Timeout = timeout });

        // Grid stable: dùng cột Code và match exact bằng regex (tránh dính code prefix).
        var exactCodeCell = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(expectedCode)}$")
        });
        await Assertions.Expect(exactCodeCell.First).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    public async Task EnsureDescriptionInGridAsync(string expectedDescription)
    {
        var timeout = _settings.StandardTimeoutMs;
        var descAnchor = _page.Locator($"td[data-caption='Description'] a[title='{expectedDescription}']");
        if (await descAnchor.CountAsync() > 0)
        {
            await Assertions.Expect(descAnchor.First).ToBeVisibleAsync(new() { Timeout = timeout });
            return;
        }

        // Fallback: description hiển thị dạng text (có thể bị rút gọn), so sánh exact bằng regex.
        var exactDescCell = _page.Locator("td[data-caption='Description']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(expectedDescription)}$")
        });
        await Assertions.Expect(exactDescCell.First).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    /// <summary>
    /// Ensure đã xoá xong một record theo Code:
    /// - không còn cell Code exact = code.
    /// </summary>
    public async Task EnsureRecordDeletedAsync(string code)
    {
        var timeout = _settings.StandardTimeoutMs;
        var exactCodeCells = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(code)}$")
        });

        await Assertions.Expect(exactCodeCells).ToHaveCountAsync(0, new() { Timeout = timeout });
    }

    public async Task OpenNewFormAsync()
    {
        await ButtonNew.ClickAsync();
    }

    public ILocator CodeCellByCode(string code) =>
        _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(code)}$")
        });

    /// <summary>
    /// Mở form Edit bằng cách click vào cell Code trong grid.
    /// </summary>
    public async Task OpenEditFormByCodeAsync(string code)
    {
        var cell = CodeCellByCode(code).First;
        await cell.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Nút Action (mũi tên dropdown) trên toolbar/grid.
    /// </summary>
    public ILocator ActionDropdownButton =>
        _page.Locator("button.dxbl-btn-primary.dxbl-btn-split-dropdown");

    /// <summary>
    /// Chọn row theo Code, click vào cột Description để focus/tick row, sau đó mở menu Action.
    /// </summary>
    public async Task OpenActionMenuForCodeAsync(string code)
    {
        var codeCell = CodeCellByCode(code).First;
        var row = codeCell.Locator("xpath=ancestor::tr[1]");

        // Click vào Description cell (theo logic cũ) để select row.
        var descriptionCell = row.Locator("td[data-caption='Description']");
        if (await descriptionCell.CountAsync() > 0)
        {
            await descriptionCell.First.ClickAsync();
        }
        else
        {
            // Fallback: td[2] thường là Description (Code = td[1]).
            await row.Locator("td").Nth(1).ClickAsync();
        }

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await ActionDropdownButton.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}

