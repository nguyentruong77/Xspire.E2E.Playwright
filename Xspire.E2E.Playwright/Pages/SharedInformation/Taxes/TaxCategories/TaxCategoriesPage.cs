using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.TaxCategories;

/// <summary>
/// Represents the Tax Categories screen under Shared Information &gt; Taxes.
/// Currently contains only basic locators and navigation helpers.
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
    /// Simple assertion helper to ensure we are on the Tax Categories page.
    /// </summary>
    public async Task EnsureOnTaxCategoriesPageAsync()
    {
        await Assertions.Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*TaxCategories.*"));
    }

    // Buttons on list screen
    // New button - ưu tiên locator theo thuộc tính ổn định thay vì XPath tuyệt đối
    public ILocator ButtonNew =>
        _page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "New" }).First;
    public ILocator ButtonEdit => _page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Edit" }).First;

    /// <summary>Opens the New Tax Category form (modal or new screen).</summary>
    public async Task OpenNewFormAsync()
    {
        await ButtonNew.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>Opens the Edit form (e.g. first row Edit button). Call after ensuring list has data.</summary>
    public async Task OpenEditFormAsync()
    {
        await ButtonEdit.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}

