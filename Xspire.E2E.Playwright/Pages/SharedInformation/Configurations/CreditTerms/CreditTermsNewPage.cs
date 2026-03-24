using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.CreditTerms;

/// <summary>
/// Shared Information &gt; Configurations &gt; Credit Terms (new/edit form).
/// </summary>
public class CreditTermsNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public CreditTermsNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/CreditTerms/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/CreditTerms/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    public ILocator InputCode =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Code *" });

    public ILocator InputDescription =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Description *" });

    public ILocator CheckboxActive =>
        _page.GetByRole(AriaRole.Checkbox, new() { Name = "Active" });

    private ILocator ApplyToFormItem =>
        _page.Locator("dxbl-form-layout-item")
            .Filter(new LocatorFilterOptions { HasTextString = "Apply To *" });

    private ILocator ApplyToDropDownButton =>
        ApplyToFormItem.GetByLabel("Open or close the drop-down");

    /// <summary>
    /// Second &quot;Open or close the drop-down&quot; on the form (Due Date Type *); first is Apply To.
    /// </summary>
    private ILocator DueDateTypeDropDownButton =>
        _page.GetByRole(AriaRole.Button, new() { Name = "Open or close the drop-down" }).Nth(1);

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
        await InputCode.FillAsync(CreditTermsTestData.CreateValid.Code);
        await InputDescription.FillAsync(CreditTermsTestData.CreateValid.Description);

        await SelectApplyToAllAsync();
        await EnsureActiveCheckedAsync();

        await SelectDueDateTypeFixedNumberOfDaysAsync();
        await FillDueDayAsync(CreditTermsTestData.CreateValid.DueDay);
    }

    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);

    private async Task EnsureActiveCheckedAsync()
    {
        if (!await CheckboxActive.IsCheckedAsync())
            await CheckboxActive.CheckAsync();
    }

    private async Task SelectApplyToAllAsync()
    {
        await ApplyToDropDownButton.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await ApplyToDropDownButton.First.ClickAsync();
        var optionAll = _page.GetByRole(AriaRole.Option,
            new() { Name = CreditTermsTestData.CreateValid.ApplyToSearchText, Exact = true });
        await optionAll.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await optionAll.First.ClickAsync();
    }

    private async Task SelectDueDateTypeFixedNumberOfDaysAsync()
    {
        await DueDateTypeDropDownButton.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await DueDateTypeDropDownButton.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        var optionExact = _page.GetByRole(AriaRole.Option,
            new() { Name = CreditTermsTestData.CreateValid.DueDateTypeOptionText, Exact = true });
        if (await optionExact.CountAsync() > 0)
        {
            await optionExact.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = _settings.StandardTimeoutMs
            });
            await optionExact.First.ClickAsync();
            return;
        }

        var visibleOptions = _page.Locator("[role='option']:visible, .dxbl-list-box-item:visible, .dxbl-list-item:visible");
        if (await visibleOptions.CountAsync() > 0)
        {
            await visibleOptions.First.ClickAsync();
            return;
        }

        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.Keyboard.PressAsync("Enter");
    }

    private async Task FillDueDayAsync(string value)
    {
        var spin = _page.GetByRole(AriaRole.Spinbutton, new() { Name = "Due Day" });
        ILocator editor;
        if (await spin.CountAsync() > 0)
            editor = spin.First;
        else
        {
            var textbox = _page.GetByRole(AriaRole.Textbox, new() { Name = "Due Day" });
            if (await textbox.CountAsync() > 0)
                editor = textbox.First;
            else
            {
                editor = _page.Locator("dxbl-form-layout-item")
                    .Filter(new LocatorFilterOptions { HasTextString = "Due Day" })
                    .Locator("input")
                    .First;
            }
        }

        await editor.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await editor.ClickAsync();
        await editor.PressAsync("Control+A");
        await editor.PressAsync("Backspace");
        await editor.FillAsync(value);
    }
}
