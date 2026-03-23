using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.Taxes;

/// <summary>
/// Shared Information > Taxes > Taxes (new/edit form).
/// </summary>
public class TaxNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public TaxNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/Taxes/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/Taxes/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    public ILocator InputCode =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Code *" });

    public ILocator InputDescription =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Description *" });

    private ILocator TaxTypeCombo =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[4]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[2]/div/dxbl-combo-box");

    private ILocator CalculationRuleCombo =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[4]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[3]/div/dxbl-combo-box");

    private ILocator TaxTypeDropDownButton =>
        _page.Locator("dxbl-form-layout-item")
            .Filter(new LocatorFilterOptions { HasTextString = "Tax Type *" })
            .GetByLabel("Open or close the drop-down");

    private ILocator CalculationRuleDropDownButton =>
        _page.Locator("dxbl-form-layout-item")
            .Filter(new LocatorFilterOptions { HasTextString = "Calculation Rule *" })
            .GetByLabel("Open or close the drop-down");

    private ILocator TaxSchedulesPanel => _page.GetByLabel("Tax Schedules");
    private ILocator TaxCategoryDetailsPanel => _page.GetByLabel("Tax Category Details");

    private ILocator TaxSchedulesAddRow =>
        TaxSchedulesPanel.GetByText("Click here to add a new row");

    private ILocator TaxCategoryDetailsAddRow =>
        TaxCategoryDetailsPanel.GetByText("Click here to add a new row");

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
        // Ensure any in-grid editors commit value before save.
        await _page.Keyboard.PressAsync("Enter");
        await _page.WaitForTimeoutAsync(150);

        if (await VisibleSaveButton.CountAsync() > 0)
        {
            await VisibleSaveButton.First.ClickAsync();
            await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            return;
        }

        // Fallback to generic toolbar button strategy.
        await ClickToolbarButtonAsync("Save");

        // Last fallback if toolbar click is blocked by overlay/editor focus.
        await _page.Keyboard.PressAsync("Control+S");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task FillValidDataAsync()
    {
        await InputCode.FillAsync(TaxesTestData.CreateValid.Code);
        await InputDescription.FillAsync(TaxesTestData.CreateValid.Description);

        await SelectFirstTaxTypeAsync();
        await SelectFirstCalculationRuleAsync();

        await FillTaxSchedulesAsync(
            TaxesTestData.CreateValid.TaxScheduleDate,
            TaxesTestData.CreateValid.TaxScheduleRate);

        await FillTaxCategoryDetailsAsync(TaxesTestData.CreateValid.TaxCategorySearchText);
    }

    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);

    private async Task SelectFirstTaxTypeAsync()
    {
        await SelectFirstItemFromOpenedDropdownAsync(TaxTypeDropDownButton, TaxTypeCombo.Locator("input").First, null);
    }

    private async Task SelectFirstCalculationRuleAsync()
    {
        await SelectFirstItemFromOpenedDropdownAsync(
            CalculationRuleDropDownButton,
            null,
            TaxesTestData.CreateValid.CalculationRuleSearchText);
    }

    private async Task SelectFirstItemFromOpenedDropdownAsync(
        ILocator dropDownButton,
        ILocator? comboInput,
        string? searchText)
    {
        await dropDownButton.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await dropDownButton.First.ClickAsync();

        if (!string.IsNullOrWhiteSpace(searchText) && comboInput is not null)
        {
            await comboInput.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = _settings.StandardTimeoutMs
            });
            await comboInput.ClickAsync();
            await comboInput.PressAsync("Control+A");
            await comboInput.PressAsync("Backspace");
            await comboInput.FillAsync(searchText);
        }

        await _page.WaitForTimeoutAsync(500);

        var visibleOptions = _page.Locator("[role='option']:visible, .dxbl-list-box-item:visible, .dxbl-list-item:visible");
        if (await visibleOptions.CountAsync() > 0)
        {
            await visibleOptions.First.ClickAsync();
            return;
        }

        var anyOption = _page.Locator(".dxbl-list-box-item, .dxbl-list-item, [role='option']").First;
        if (await anyOption.CountAsync() > 0)
        {
            await anyOption.ClickAsync(new LocatorClickOptions { Force = true });
            return;
        }

        // Last fallback: if popup captured focus, select first item by keyboard.
        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.Keyboard.PressAsync("Enter");
    }

    private async Task FillTaxSchedulesAsync(string startDate, string taxRate)
    {
        await TaxSchedulesAddRow.ClickAsync();
        await _page.WaitForTimeoutAsync(500);

        // Prefer role-based grid cells so we do not click virtual spacer rows.
        var startDateCell = TaxSchedulesPanel.GetByRole(AriaRole.Gridcell, new() { Name = "Start Date *" });
        if (await startDateCell.CountAsync() > 0)
        {
            await startDateCell.First.ClickAsync();
        }
        else
        {
            var startDateHeaderCell = TaxSchedulesPanel.Locator("td[data-caption='Start Date *']").First;
            if (await startDateHeaderCell.CountAsync() > 0)
                await startDateHeaderCell.ClickAsync();
        }

        await _page.WaitForTimeoutAsync(250);

        // After add-row, Start Date is pre-filled with current date => clear then type required value.
        var activeEditor = _page.Locator("input:focus, textarea:focus").First;
        if (await activeEditor.CountAsync() > 0)
        {
            await activeEditor.PressAsync("Control+A");
            await activeEditor.PressAsync("Backspace");
            await activeEditor.FillAsync(startDate);
            await activeEditor.PressAsync("Tab");
        }
        else
        {
            // Fallback when focus is not preserved.
            var scheduleInputs = TaxSchedulesPanel.Locator("input:visible, textarea:visible");
            await scheduleInputs.First.ClickAsync();
            await scheduleInputs.First.PressAsync("Control+A");
            await scheduleInputs.First.PressAsync("Backspace");
            await scheduleInputs.First.FillAsync(startDate);
            await scheduleInputs.First.PressAsync("Tab");
        }

        await _page.WaitForTimeoutAsync(200);

        // Fill % Tax Rate in current active editor (after tab from Start Date).
        var rateEditor = _page.Locator("input:focus, textarea:focus").First;
        if (await rateEditor.CountAsync() > 0)
        {
            await rateEditor.PressAsync("Control+A");
            await rateEditor.PressAsync("Backspace");
            await rateEditor.FillAsync(taxRate);
            return;
        }

        // Final fallback: second visible input in the Tax Schedules section.
        var visibleInputs = TaxSchedulesPanel.Locator("input:visible, textarea:visible");
        if (await visibleInputs.CountAsync() >= 2)
        {
            await visibleInputs.Nth(1).ClickAsync();
            await visibleInputs.Nth(1).PressAsync("Control+A");
            await visibleInputs.Nth(1).PressAsync("Backspace");
            await visibleInputs.Nth(1).FillAsync(taxRate);
        }
    }

    private async Task FillTaxCategoryDetailsAsync(string taxCategorySearchText)
    {
        await TaxCategoryDetailsAddRow.ClickAsync();
        await _page.WaitForTimeoutAsync(400);

        // After clicking add-row, grid auto-focuses combobox input; use focused input first.
        var comboInput = _page.Locator("input:focus, input[role='combobox']:focus").First;
        if (await comboInput.CountAsync() == 0)
        {
            comboInput = TaxCategoryDetailsPanel
                .Locator("input[role='combobox'], .dxbl-combo-box input, input")
                .First;
            await comboInput.ClickAsync();
        }

        await comboInput.PressAsync("Control+A");
        await comboInput.PressAsync("Backspace");
        await comboInput.FillAsync(taxCategorySearchText);
        await _page.WaitForTimeoutAsync(500);

        var visibleOptions = _page.Locator("[role='option']:visible, .dxbl-list-box-item:visible, .dxbl-list-item:visible");
        if (await visibleOptions.CountAsync() > 0)
        {
            await visibleOptions.First.ClickAsync();
            return;
        }

        var fallbackItem = _page.Locator(".dxbl-list-box-item, .dxbl-list-item, [role='option']").First;
        if (await fallbackItem.CountAsync() > 0)
        {
            await fallbackItem.ClickAsync(new LocatorClickOptions { Force = true });
            return;
        }

        // Last fallback: if popup focus is active, select first result by keyboard.
        await _page.Keyboard.PressAsync("ArrowDown");
        await _page.Keyboard.PressAsync("Enter");
    }
}
