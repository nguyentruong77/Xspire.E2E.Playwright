using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.ReasonCodes;

/// <summary>
/// Shared Information > Configurations > Reason Codes (new/edit form).
/// </summary>
public class ReasonCodesNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public ReasonCodesNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/ReasonCodes/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/ReasonCodes/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    public ILocator InputCode =>
        _page.Locator("input[name='ne_Txt_Code']").First;

    public ILocator InputDescription =>
        _page.Locator("input[name='ne_Txt_Description']").First;

    private ILocator TypeDropDownButton =>
        _page.GetByRole(AriaRole.Button, new() { Name = "Open or close the drop-down" }).First;

    private ILocator TypeComboInput =>
        _page.Locator(".dxbl-combo-box input[role='combobox']:visible, .dxbl-combo-box input:visible").First;

    private ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    private ILocator VisibleSaveButton =>
        _page.Locator("button:visible").Filter(new LocatorFilterOptions { HasTextString = "Save" });

    private async Task ClickToolbarButtonAsync(string text)
    {
        var roleButton = _page.GetByRole(AriaRole.Button, new() { Name = text }).First;
        if (await roleButton.CountAsync() > 0)
            await roleButton.ClickAsync();
        else
            await GetToolbarButton(text).First.ClickAsync();

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickSaveAsync()
    {
        await _page.Keyboard.PressAsync("Enter");
        await _page.WaitForTimeoutAsync(150);

        if (await VisibleSaveButton.CountAsync() > 0)
        {
            await VisibleSaveButton.First.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            return;
        }

        await ClickToolbarButtonAsync("Save");

        await _page.Keyboard.PressAsync("Control+S");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task FillValidDataAsync()
    {
        await InputCode.FillAsync(ReasonCodesTestData.CreateValid.Code);
        await InputDescription.FillAsync(ReasonCodesTestData.CreateValid.Description);
        await SelectTypeAsync();
    }

    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);

    private async Task SelectTypeAsync()
    {
        await TypeDropDownButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await TypeDropDownButton.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        if (await TypeComboInput.CountAsync() > 0)
        {
            await TypeComboInput.ClickAsync();
            await TypeComboInput.PressAsync("Control+A");
            await TypeComboInput.PressAsync("Backspace");
            await TypeComboInput.FillAsync(ReasonCodesTestData.CreateValid.TypeSearchText);
            await _page.WaitForTimeoutAsync(500);
        }

        var issueOption = _page.GetByRole(AriaRole.Option, new() { Name = ReasonCodesTestData.CreateValid.TypeOptionText, Exact = true });
        if (await issueOption.CountAsync() > 0)
        {
            await issueOption.First.ClickAsync();
            return;
        }

        var firstVisibleOption = _page.Locator("[role='option']:visible, .dxbl-list-box-item:visible, .dxbl-list-item:visible");
        if (await firstVisibleOption.CountAsync() > 0)
        {
            await firstVisibleOption.First.ClickAsync();
            return;
        }

        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.Keyboard.PressAsync("Enter");
    }
}
