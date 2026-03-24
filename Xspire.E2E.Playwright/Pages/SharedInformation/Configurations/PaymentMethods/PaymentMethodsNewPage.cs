using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.PaymentMethods;

/// <summary>
/// Shared Information &gt; Configurations &gt; Payment Methods (new/edit form).
/// </summary>
public class PaymentMethodsNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public PaymentMethodsNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/PaymentMethods/00000000-0000-0000-0000-000000000000.*"));
    }

    public async Task EnsureOnEditPageAsync(string expectedCode)
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*SharedInformation/PaymentMethods/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });

        await Assertions.Expect(InputPaymentMethodCode).ToHaveValueAsync(expectedCode);
    }

    public ILocator InputPaymentMethodCode =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Payment Method Code *" });

    public ILocator InputMeansOfPayment =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Means Of Payment" });

    public ILocator InputDescription =>
        _page.GetByRole(AriaRole.Textbox, new() { Name = "Description *" });

    public ILocator CheckboxUseInAp =>
        _page.GetByRole(AriaRole.Checkbox, new() { Name = "Use In AP" });

    public ILocator CheckboxUseInAr =>
        _page.GetByRole(AriaRole.Checkbox, new() { Name = "Use In AR" });

    public ILocator CheckboxUseInPr =>
        _page.GetByRole(AriaRole.Checkbox, new() { Name = "Use In PR" });

    public ILocator CheckboxActive =>
        _page.GetByRole(AriaRole.Checkbox, new() { Name = "Active" });

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
        await InputPaymentMethodCode.FillAsync(PaymentMethodsTestData.CreateValid.Code);
        await InputMeansOfPayment.FillAsync(PaymentMethodsTestData.CreateValid.MeansOfPayment);
        await InputDescription.FillAsync(PaymentMethodsTestData.CreateValid.Description);

        await EnsureActiveCheckedAsync();
    }

    public Task FillDescriptionOnlyAsync(string description) => InputDescription.FillAsync(description);

    private async Task EnsureActiveCheckedAsync()
    {
        if (!await CheckboxActive.IsCheckedAsync())
            await CheckboxActive.CheckAsync();
    }
}
