using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.Attributes;

/// <summary>
/// Shared Information &gt; Configurations &gt; Attributes (new/edit form).
/// </summary>
public class AttributeNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public AttributeNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/Attributes/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/Attributes/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    public ILocator InputCode =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Code *" });

    public ILocator InputDescription =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Description *" });

    public ILocator InputEntryMask =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Entry Mask" });

    public ILocator InputRegExp =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Reg Exp" });

    private ILocator ControlTypeFormItem =>
        _page.Locator("dxbl-form-layout-item")
            .Filter(new LocatorFilterOptions { HasTextString = "Control Type *" });

    private ILocator ControlTypeDropDownButton =>
        ControlTypeFormItem.GetByLabel("Open or close the drop-down");

    private ILocator ControlTypeComboInput =>
        ControlTypeFormItem.Locator(".dxbl-combo-box input, input[role='combobox']").First;

    private ILocator ValuesAddRow =>
        _page.GetByText("Click here to add a new row", new() { Exact = false });

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

    public async Task FillEntryMaskAsync(string value) => await InputEntryMask.FillAsync(value);

    public async Task FillRegExpAsync(string value) => await InputRegExp.FillAsync(value);

    public async Task FillEntryMaskAndRegExpIfProvidedAsync(string? entryMask, string? regExp)
    {
        if (!string.IsNullOrWhiteSpace(entryMask))
            await InputEntryMask.FillAsync(entryMask);
        if (!string.IsNullOrWhiteSpace(regExp))
            await InputRegExp.FillAsync(regExp);
    }

    public async Task FillValidDataAsync()
    {
        await InputCode.FillAsync(AttributesTestData.CreateValid.Code);
        await SelectControlTypeCheckboxAsync();
        await InputDescription.FillAsync(AttributesTestData.CreateValid.Description);
        await FillCheckboxCreateValueRowsAsync();
    }

    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);

    public async Task SelectControlTypeCheckboxAsync()
    {
        await ControlTypeDropDownButton.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await ControlTypeDropDownButton.First.ClickAsync();
        await _page.WaitForTimeoutAsync(200);

        if (await ControlTypeComboInput.CountAsync() > 0)
        {
            await ControlTypeComboInput.ClickAsync();
            await ControlTypeComboInput.PressAsync("Control+A");
            await ControlTypeComboInput.PressAsync("Backspace");
            await ControlTypeComboInput.FillAsync(AttributesTestData.CreateValid.ControlTypeSearchText);
        }

        await _page.WaitForTimeoutAsync(500);

        var option = _page.GetByRole(AriaRole.Option, new() { Name = AttributesTestData.CreateValid.ControlTypeOptionText });
        if (await option.CountAsync() > 0)
        {
            await option.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = _settings.StandardTimeoutMs
            });
            await option.First.ClickAsync();
            return;
        }

        var optionFilter = _page.GetByRole(AriaRole.Option)
            .Filter(new LocatorFilterOptions { HasTextString = AttributesTestData.CreateValid.ControlTypeOptionText });
        if (await optionFilter.CountAsync() > 0)
        {
            await optionFilter.First.ClickAsync();
            return;
        }

        var listItem = _page.Locator(".dxbl-list-box-item, .dxbl-list-item")
            .Filter(new LocatorFilterOptions { HasTextString = AttributesTestData.CreateValid.ControlTypeOptionText });
        if (await listItem.CountAsync() > 0)
        {
            await listItem.First.ClickAsync();
            return;
        }

        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.Keyboard.PressAsync("Enter");
    }

    public async Task FillCheckboxCreateValueRowsAsync()
    {
        await ValuesAddRow.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await FillNewValueRowAsync(
            AttributesTestData.CreateValid.ValueRow1Code,
            AttributesTestData.CreateValid.ValueRow1Description,
            AttributesTestData.CreateValid.ValueRow1SortOrder);

        await BlurValueGridAsync();

        await ValuesAddRow.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await FillNewValueRowAsync(
            AttributesTestData.CreateValid.ValueRow2Code,
            AttributesTestData.CreateValid.ValueRow2Description,
            AttributesTestData.CreateValid.ValueRow2SortOrder);

        await BlurValueGridAsync();
    }

    private async Task FillNewValueRowAsync(string valueCode, string description, string sortOrder)
    {
        await ValuesAddRow.ClickAsync();
        await _page.WaitForTimeoutAsync(450);

        // Sau add-row, màn hình tự focus vào Value Code — không click cell trừ khi focus lệch.
        await FillValueCodeAfterAddRowAsync(valueCode);
        await _page.Keyboard.PressAsync("Tab");
        await _page.WaitForTimeoutAsync(150);

        await FillFocusedInputAsync(description);
        await _page.Keyboard.PressAsync("Tab");
        await _page.WaitForTimeoutAsync(150);

        await FillFocusedInputAsync(sortOrder);
    }

    private async Task FillValueCodeAfterAddRowAsync(string value)
    {
        if (await TryFillIntoFocusedInputAsync(value))
            return;

        await FocusValueCodeEditorFallbackAsync();

        if (await TryFillIntoFocusedInputAsync(value))
            return;

        await FillIntoLastVisibleInputAsync(value);
    }

    private async Task FillFocusedInputAsync(string value)
    {
        if (await TryFillIntoFocusedInputAsync(value))
            return;

        await FillIntoLastVisibleInputAsync(value);
    }

    private static async Task FillIntoLastVisibleInputAsync(IPage page, string value)
    {
        var fallback = page.Locator("input:visible, textarea:visible").Last;
        await fallback.ClickAsync();
        await fallback.PressAsync("Control+A");
        await fallback.PressAsync("Backspace");
        await fallback.FillAsync(value);
    }

    private Task FillIntoLastVisibleInputAsync(string value) =>
        FillIntoLastVisibleInputAsync(_page, value);

    private async Task<bool> TryFillIntoFocusedInputAsync(string value)
    {
        var active = _page.Locator("input:focus, textarea:focus").First;
        if (await active.CountAsync() == 0)
            return false;

        await active.PressAsync("Control+A");
        await active.PressAsync("Backspace");
        await active.FillAsync(value);
        return true;
    }

    private async Task FocusValueCodeEditorFallbackAsync()
    {
        var valueCodeCell = _page.GetByRole(AriaRole.Gridcell, new() { Name = "Value Code *" });
        if (await valueCodeCell.CountAsync() > 0)
        {
            await valueCodeCell.Last.ClickAsync();
            await _page.WaitForTimeoutAsync(200);
            return;
        }

        var td = _page.Locator("td[data-caption='Value Code *'], td[data-caption='Value Code']").Last;
        if (await td.CountAsync() > 0)
        {
            await td.ClickAsync(new LocatorClickOptions { Timeout = _settings.StandardTimeoutMs });
            await _page.WaitForTimeoutAsync(200);
        }
    }

    private async Task BlurValueGridAsync()
    {
        await InputDescription.ClickAsync();
        await _page.WaitForTimeoutAsync(350);
    }
}
