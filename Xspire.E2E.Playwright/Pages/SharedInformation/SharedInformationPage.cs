using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation;

/// <summary>
/// Represents the Shared Information workspace page (Workspace/SI) that shows shortcut links.
/// </summary>
public class SharedInformationPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public SharedInformationPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Header "Shared Information" tab - breadcrumb; consider asserting by URL if multi-locale.
    public ILocator SharedInformationTab =>
        _page.Locator("text=Shared Information");

    // Taxes group - "Tax Categories" shortcut (locale-agnostic: match by route)
    public ILocator TaxCategoriesLink =>
        _page.Locator("a[href*='TaxCategories']");

    // Taxes group - "Taxes" shortcut (locale-agnostic: match by route, exclude TaxCategories)
    public ILocator TaxesLink =>
        _page.Locator("a[href*='Taxes']:not([href*='TaxCategories'])");

    /// <summary>
    /// Navigate from Shared Information workspace to Tax Categories screen.
    /// Assumes caller has already navigated to Workspace/SI.
    /// </summary>
    public async Task NavigateToTaxCategoriesAsync()
    {
        await TaxCategoriesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Navigate from Shared Information workspace to Taxes screen.
    /// Assumes caller has already navigated to Workspace/SI.
    /// </summary>
    public async Task NavigateToTaxesAsync()
    {
        await TaxesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}

