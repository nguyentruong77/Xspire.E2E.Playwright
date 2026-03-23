using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.TaxCategories;

/// <summary>
/// Shared Information &gt; Taxes &gt; Tax Categories (list).
/// </summary>
public class TaxCategoriesPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public TaxCategoriesPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Page title/header - locale-dependent; prefer asserting by URL (EnsureOnTaxCategoriesPageAsync).
    public ILocator PageTitle =>
        _page.Locator("h1, h2").Filter(new LocatorFilterOptions { HasTextString = "Tax Categories" });

    /// <summary>
    /// Simple assertion helper to ensure we are on the Tax Categories page (list or detail).
    /// </summary>
    public async Task EnsureOnTaxCategoriesPageAsync()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*TaxCategories.*"));
    }

    /// <summary>
    /// Luôn Goto về màn list Tax Categories và chờ toolbar sẵn sàng.
    /// </summary>
    public async Task EnsureOnTaxCategoriesListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/SharedInformation/TaxCategories");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/TaxCategories.*"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>
    /// Sau Save: URL detail có GUID thật (không all-zero).
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*TaxCategories/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    public ILocator ButtonNew => GetToolbarButton("New");

    public async Task OpenNewFormAsync()
    {
        await ButtonNew.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task FillSearchAsync(string code)
    {
        var textbox = _page.GetByRole(AriaRole.Textbox).First;
        await textbox.ClickAsync();
        await textbox.FillAsync(code);
        await textbox.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task EnsureSearchSuccessAsync(string expectedCode)
    {
        var timeout = _settings.StandardTimeoutMs;
        var textbox = _page.GetByRole(AriaRole.Textbox).First;
        await Assertions.Expect(textbox).ToHaveValueAsync(expectedCode, new() { Timeout = timeout });

        var exactCodeCell = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(expectedCode)}$")
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

        var exactDescCell = _page.Locator("td[data-caption='Description']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(expectedDescription)}$")
        });
        await Assertions.Expect(exactDescCell.First).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    public async Task EnsureRecordDeletedAsync(string code)
    {
        var timeout = _settings.StandardTimeoutMs;
        var exactCodeCells = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(code)}$")
        });

        await Assertions.Expect(exactCodeCells).ToHaveCountAsync(0, new() { Timeout = timeout });
    }

    public ILocator CodeCellByCode(string code) =>
        _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(code)}$")
        });

    public async Task OpenEditFormByCodeAsync(string code)
    {
        var cell = CodeCellByCode(code).First;
        await cell.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public ILocator ActionDropdownButton =>
        _page.Locator("button.dxbl-btn-primary.dxbl-btn-split-dropdown");

    public async Task OpenActionMenuForCodeAsync(string code)
    {
        var codeCell = CodeCellByCode(code).First;
        var row = codeCell.Locator("xpath=ancestor::tr[1]");

        var descriptionCell = row.Locator("td[data-caption='Description']");
        if (await descriptionCell.CountAsync() > 0)
        {
            await descriptionCell.First.ClickAsync();
        }
        else
        {
            await row.Locator("td").Nth(1).ClickAsync();
        }

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await ActionDropdownButton.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}

