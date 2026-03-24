using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.PaymentMethods;

/// <summary>
/// Shared Information &gt; Configurations &gt; Payment Methods (list).
/// List cột định danh dùng caption đầy đủ (vd. Payment Method Code), không dùng chữ &quot;Code&quot; thuần — xem docs/Plan.md mục 5–6.
/// </summary>
public class PaymentMethodsPage
{
    /// <summary>Giá trị <c>data-caption</c> cột đầu (mở Edit).</summary>
    public const string IdentityColumnCaption = "Payment Method Code";

    /// <summary>Giá trị <c>data-caption</c> cột thứ hai (chọn dòng trước Action / Delete).</summary>
    public const string RowSelectColumnCaption = "Means Of Payment";

    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public PaymentMethodsPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnPaymentMethodsPageAsync()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*SharedInformation/PaymentMethods.*"));
    }

    public async Task EnsureOnPaymentMethodsListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/SharedInformation/PaymentMethods");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/PaymentMethods.*"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/PaymentMethods/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    public ILocator ButtonNew => GetToolbarButton("New");

    public async Task OpenNewFormAsync()
    {
        await ButtonNew.First.ClickAsync();
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

        var exactCodeCell = _page.Locator($"td[data-caption='{IdentityColumnCaption}']").Filter(new LocatorFilterOptions
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
        var exactCodeCells = _page.Locator($"td[data-caption='{IdentityColumnCaption}']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(code)}$")
        });

        await Assertions.Expect(exactCodeCells).ToHaveCountAsync(0, new() { Timeout = timeout });
    }

    public ILocator CodeCellByCode(string code) =>
        _page.Locator($"td[data-caption='{IdentityColumnCaption}']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(code)}$")
        });

    public async Task OpenEditFormByCodeAsync(string code)
    {
        var cell = CodeCellByCode(code).First;
        var link = cell.Locator("a");
        if (await link.CountAsync() > 0)
            await link.First.ClickAsync();
        else
            await cell.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public ILocator ActionDropdownButton =>
        _page.Locator("button.dxbl-btn-primary.dxbl-btn-split-dropdown");

    public async Task OpenActionMenuForCodeAsync(string code)
    {
        var codeCell = CodeCellByCode(code).First;
        var row = codeCell.Locator("xpath=ancestor::tr[1]");

        var secondColumnCell = row.Locator($"td[data-caption='{RowSelectColumnCaption}']");
        if (await secondColumnCell.CountAsync() > 0)
            await secondColumnCell.First.ClickAsync();
        else
            await row.Locator("td").Nth(1).ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await ActionDropdownButton.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
