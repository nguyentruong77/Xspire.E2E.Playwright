using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Taxes;

/// <summary>
/// Màn form New Tax Category (màn riêng, URL có GUID placeholder toàn 0).
/// </summary>
public class TaxCategoryNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public TaxCategoryNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    /// <summary>
    /// Ensure đã vào màn New: URL chứa TaxCategories/00000000-0000-0000-0000-000000000000 (locale-agnostic).
    /// </summary>
    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*TaxCategories/00000000-0000-0000-0000-000000000000.*"));
    }

    // Tab/breadcrumb "Tax Categories - New" (chỉ đúng khi dùng tiếng Anh)
    public ILocator TabNew => _page.GetByText("Tax Categories - New", new() { Exact = true });

    // Form fields – dùng name cố định (locale-agnostic), không dùng id vì id sinh GUID mỗi lần
    public ILocator InputCode => _page.Locator("input[name='ne_Txt_Code']");
    public ILocator InputDescription => _page.Locator("input[name='ne_Txt_Description']");

    public ILocator ButtonSave => _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).First;
    public ILocator ButtonCancel => _page.GetByRole(AriaRole.Button, new() { Name = "Cancel" }).First;

    public async Task ClickSaveAsync()
    {
        await ButtonSave.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickCancelAsync()
    {
        await ButtonCancel.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
