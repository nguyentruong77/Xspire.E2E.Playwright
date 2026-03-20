using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.CustomerClassification.Locations;

/// <summary>
/// Represents the Locations - New/Edit form page.
/// </summary>
public class LocationsNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public LocationsNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(
                ".*Locations/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/Locations/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode)
            .ToHaveValueAsync(expectedCode, new() { Timeout = _settings.StandardTimeoutMs });
    }

    // ===== Required fields =====
    // User requested selector based on accessible name (DevExpress required fields show label "*").
    public ILocator InputCode => _page.GetByRole(AriaRole.Textbox, new() { Name = "Code *" }).First;

    public ILocator InputDescription =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Description *" }).First;

    // ===== Toolbar buttons (Back/Save) =====
    private ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    private async Task ClickToolbarButtonAsync(string text)
    {
        // Prefer stable role-based locator (if available).
        var roleButton = _page.GetByRole(AriaRole.Button, new() { Name = text }).First;
        if (await roleButton.CountAsync() > 0)
        {
            await roleButton.ClickAsync();
        }
        else
        {
            await GetToolbarButton(text).First.ClickAsync();
        }

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public Task ClickSaveAsync() => ClickToolbarButtonAsync("Save");

    public Task ClickBackAsync() => ClickToolbarButtonAsync("Back");

    /// <summary>
    /// Fill required fields for create success.
    /// </summary>
    public async Task FillRequiredFieldsAsync(string code, string description)
    {
        await InputCode.FillAsync(code);
        await Assertions.Expect(InputCode).ToHaveValueAsync(code, new() { Timeout = _settings.StandardTimeoutMs });

        await InputDescription.FillAsync(description);
        await Assertions.Expect(InputDescription)
            .ToHaveValueAsync(description, new() { Timeout = _settings.StandardTimeoutMs });
    }

    /// <summary>
    /// Edit only Description.
    /// </summary>
    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);
}

