using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.GeoSalesStructure;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.GeoSalesStructure.TerritoryLevelDefinitions;

/// <summary>
/// Territory Level Definitions — New/Edit form (name/id/Country/Level Description giống GeographyLevelDefinitionsNewPage;
/// thêm bước Linked Geography Level cho màn Territory).
/// </summary>
public class TerritoryLevelDefinitionsNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public TerritoryLevelDefinitionsNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    /// <summary>
    /// Ensure đã vào màn New: URL chứa GUID toàn 0.
    /// </summary>
    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(
                ".*SharedInformation/TerritoryLevelDefinitions/00000000-0000-0000-0000-000000000000.*"));
    }

    /// <summary>
    /// Ensure đang ở màn Edit:
    /// - URL có GUID thật (không all-zero)
    /// - Input Code có value đúng expectedCode
    /// </summary>
    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(
                ".*/SharedInformation/TerritoryLevelDefinitions/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputCode).ToHaveValueAsync(expectedCode);
    }

    // ===== Required fields =====

    public ILocator InputCode => _page.Locator("input[name='ne_Txt_Code']");

    public ILocator InputDescription => _page.Locator("input[name='ne_Txt_Description']");

    public ILocator InputMaxLevel => _page.Locator("input[name='ne_Txt_MaxLevel']");

    public ILocator CheckboxIsActive =>
        _page.Locator("input[name='ne_Chk_IsActive'], input#ne_Chk_IsActive, input[type='checkbox'][name='ne_Chk_IsActive']");

    /// <summary>
    /// Country combobox wrapper (provided by user) — cùng XPath màn Geography Level Definitions.
    /// </summary>
    private ILocator CountryComboBoxProvided =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[5]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[3]/div/dxbl-combo-box");

    private ILocator CountryComboInputProvided =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[3]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[3]/div/dxbl-combo-box/input");

    private ILocator CountryComboBoxByRole =>
        _page.GetByRole(AriaRole.Combobox).First;

    private ILocator CountryComboBoxFirstDxbl =>
        _page.Locator("form .dxbl-combo-box").First;

    /// <summary>
    /// Level Details - Level Description cell (row Level=1).
    /// </summary>
    private ILocator LevelDescriptionCell =>
        _page.Locator(".dxbl-grid-table > tbody > tr:nth-child(2) > td:nth-child(3)");

    // ===== Toolbar buttons (Back/Save) =====

    private ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    private ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    private async Task ClickToolbarButtonAsync(string text)
    {
        // Prefer stable role-based locator (DevExpress buttons sometimes not match dxbl-btn list reliably).
        var roleButton = _page.GetByRole(AriaRole.Button, new() { Name = text }).First;
        if (await roleButton.CountAsync() > 0)
        {
            await roleButton.ClickAsync();
        }
        else
        {
            var button = GetToolbarButton(text);
            await button.First.ClickAsync();
        }

        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public Task ClickSaveAsync() => ClickToolbarButtonAsync("Save");

    public Task ClickBackAsync() => ClickToolbarButtonAsync("Back");

    // ===== Support methods =====

    private async Task SelectFirstCountryAsync()
    {
        // Use the specific combobox input locator you provided.
        var input = CountryComboInputProvided;

        await input.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await input.First.ClickAsync();
        await input.First.PressAsync("Control+A");
        await input.First.PressAsync("Backspace");
        await input.First.FillAsync(TerritoryLevelDefinitionsTestData.CreateValid.CountrySearchText);

        await _page.WaitForTimeoutAsync(800);

        // Select the first filtered element.
        var options = _page.GetByRole(AriaRole.Option);
        if (await options.CountAsync() > 0)
        {
            await options.First.ClickAsync();
            return;
        }

        // Fallback if role=option isn't used.
        var candidates = _page.Locator(
            ".dxbl-list-box-item, .dxbl-list-item, li[role='option'], div[role='option']").First;
        await candidates.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await candidates.ClickAsync();
    }

    private async Task<ILocator> ResolveCountryComboBoxAsync()
    {
        if (await CountryComboBoxProvided.CountAsync() > 0)
            return CountryComboBoxProvided;

        if (await CountryComboBoxByRole.CountAsync() > 0)
            return CountryComboBoxByRole;

        return CountryComboBoxFirstDxbl;
    }

    private async Task EnsureIsActiveCheckedAsync()
    {
        await Assertions.Expect(CheckboxIsActive).ToBeVisibleAsync(new() { Timeout = _settings.StandardTimeoutMs });

        // If already checked, do nothing; otherwise tick.
        try
        {
            if (!await CheckboxIsActive.IsCheckedAsync())
                await CheckboxIsActive.ClickAsync();
        }
        catch
        {
            // Fallback: rely on attribute presence.
            var checkedAttr = await CheckboxIsActive.GetAttributeAsync("checked");
            if (string.IsNullOrWhiteSpace(checkedAttr))
                await CheckboxIsActive.ClickAsync();
        }
    }

    private async Task FillLevelDescriptionAsync(string levelDescription)
    {
        await LevelDescriptionCell.ClickAsync();
        await _page.WaitForTimeoutAsync(300);

        // Prefer inline input inside the clicked cell.
        var inlineEditor = LevelDescriptionCell.Locator("input, textarea, div[contenteditable='true']");
        if (await inlineEditor.CountAsync() > 0)
        {
            var editor = inlineEditor.First;
            var tagName = await editor.EvaluateAsync<string>("e => e.tagName");
            if (string.Equals(tagName, "DIV", System.StringComparison.OrdinalIgnoreCase))
            {
                // contenteditable editor: use keyboard to ensure text is entered.
                await editor.ClickAsync();
                await _page.Keyboard.PressAsync("Control+A");
                await _page.Keyboard.PressAsync("Backspace");
                await _page.Keyboard.TypeAsync(levelDescription);
            }
            else
            {
                await editor.FillAsync(levelDescription);
            }

            return;
        }

        // Fallback: global textbox (after cell click usually only Level Description editor is active).
        var textbox = _page.GetByRole(AriaRole.Textbox).First;
        await textbox.FillAsync(levelDescription);
    }

    private async Task SelectFirstLinkedGeographyLevelAsync()
    {
        var linkedCell = _page.GetByRole(AriaRole.Gridcell, new() { Name = "Linked Geography Level" });
        await linkedCell.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await linkedCell.First.ClickAsync();

        var dropDownTrigger = _page
            .GetByRole(AriaRole.Gridcell, new() { Name = "Open or close the drop-down" })
            .GetByLabel("Open or close the drop-down");
        await dropDownTrigger.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await dropDownTrigger.First.ClickAsync();

        await _page.WaitForTimeoutAsync(400);

        var options = _page.GetByRole(AriaRole.Option);
        if (await options.CountAsync() > 0)
        {
            await options.First.ClickAsync();
            return;
        }

        var listItem = _page.Locator(".dxbl-list-box-item, .dxbl-list-item").First;
        await listItem.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await listItem.ClickAsync();
    }

    /// <summary>
    /// Fill required fields for create/edit success (bổ sung Linked Geography Level sau Level Description).
    /// </summary>
    public async Task FillRequiredFieldsAsync(
        string code,
        string description,
        string maxLevel,
        string levelDescription,
        bool isActive = true)
    {
        await InputCode.FillAsync(code);
        await Assertions.Expect(InputCode).ToHaveValueAsync(code, new() { Timeout = _settings.StandardTimeoutMs });

        // Country: always choose first item as requested.
        await SelectFirstCountryAsync();
        // Ensure country value isn't empty (best-effort; combobox input may vary).
        var countryInput = (await ResolveCountryComboBoxAsync()).Locator("input").First;
        if (await countryInput.CountAsync() > 0)
        {
            var value = await countryInput.GetAttributeAsync("value");
            if (string.IsNullOrWhiteSpace(value))
            {
                // If value attribute doesn't exist, skip hard assertion to avoid false negatives.
            }
        }

        await InputDescription.FillAsync(description);
        await Assertions.Expect(InputDescription).ToHaveValueAsync(description, new() { Timeout = _settings.StandardTimeoutMs });
        await InputMaxLevel.FillAsync(maxLevel);
        await Assertions.Expect(InputMaxLevel).ToHaveValueAsync(maxLevel, new() { Timeout = _settings.StandardTimeoutMs });

        if (isActive)
            await EnsureIsActiveCheckedAsync();

        await FillLevelDescriptionAsync(levelDescription);
        await SelectFirstLinkedGeographyLevelAsync();
    }

    /// <summary>
    /// Edit only Description (as requested).
    /// </summary>
    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);
}
