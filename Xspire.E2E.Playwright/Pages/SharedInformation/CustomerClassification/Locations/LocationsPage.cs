using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.CustomerClassification.Locations;

/// <summary>
/// Represents the Locations list screen.
/// </summary>
public class LocationsPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public LocationsPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // ===== Toolbar buttons (New/Back/Save) =====
    private ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    private async Task ClickToolbarButtonAsync(string text)
    {
        // Prefer stable role-based locator first.
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

    public ILocator ButtonNew => GetToolbarButton("New");

    public async Task EnsureOnLocationsListAsync()
    {
        // Keep navigation stable: always land on Locations list page.
        await GotoLocationsListAsync();

        await Assertions.Expect(_page).ToHaveURLAsync(new Regex(".*Locations.*"),
            new() { Timeout = _settings.StandardTimeoutMs });

        // Wait for New button (toolbar) to ensure list is fully ready.
        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>
    /// Luôn điều hướng (GotoAsync) về màn list Locations.
    /// Dùng sau Create/Edit để đảm bảo DOM/state sạch cho các bước search/assert tiếp theo.
    /// </summary>
    public async Task GotoLocationsListAsync()
    {
        await _page.GotoAsync("https://xspire-test.hqsoft.vn/SharedInformation/Locations");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Ensure lưu thành công dựa trên URL detail:
    /// - New form luôn có GUID all-zero placeholder
    /// - Lưu thành công chuyển sang GUID thật (không còn all-zero)
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*/Locations/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    public async Task NavigateBackToListAsync()
    {
        await ClickToolbarButtonAsync("Back");
    }

    // ===== Toolbar / search =====
    public async Task FillSearchAsync(string code)
    {
        // There may be other disabled textbox inputs on page (especially on failed navigation flows).
        // Pick the first enabled textbox instead of always using `.First`.
        var textboxes = _page.GetByRole(AriaRole.Textbox);
        var count = await textboxes.CountAsync();
        if (count == 0)
            throw new PlaywrightException("No textbox found for search.");

        ILocator? enabled = null;
        for (var i = 0; i < count; i++)
        {
            var candidate = textboxes.Nth(i);
            try
            {
                if (await candidate.IsEnabledAsync())
                {
                    enabled = candidate;
                    break;
                }
            }
            catch
            {
                // Ignore and try next textbox.
            }
        }

        enabled ??= textboxes.First;

        await enabled.ClickAsync();
        await enabled.FillAsync(code);
        await enabled.PressAsync("Enter");
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

        // Prefer full text in title attribute.
        var descAnchor = _page.Locator($"td[data-caption='Description'] a[title='{expectedDescription}']");
        if (await descAnchor.CountAsync() > 0)
        {
            await Assertions.Expect(descAnchor.First).ToBeVisibleAsync(new() { Timeout = timeout });
            return;
        }

        // Fallback: exact text on cell (can be truncated in UI).
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

    // ===== Navigation / actions =====
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

    public ILocator CodeCellByCode(string code) =>
        _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new Regex($"^{Regex.Escape(code)}$")
        });

    /// <summary>
    /// Mở form Edit bằng cách click vào cell Code trong grid.
    /// </summary>
    public async Task OpenEditFormByCodeAsync(string code)
    {
        var cell = CodeCellByCode(code).First;

        // Prefer clicking the link (if grid uses <a title="{code}">).
        // Fallback to clicking the cell itself.
        var codeLink = cell.Locator($"a[title='{code}'], a:has-text('{code}')").First;
        if (await codeLink.CountAsync() > 0)
            await codeLink.ClickAsync();
        else
            await cell.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Action (mũi tên dropdown) ở góc phải grid.
    /// </summary>
    public ILocator ActionDropdownButton =>
        _page.Locator("button.dxbl-btn-primary.dxbl-btn-split-dropdown");

    /// <summary>
    /// Logic cũ (đúng như user yêu cầu):
    /// - Click vào Description cell để chọn row
    /// - Sau đó mở Action dropdown để thao tác Delete...
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

