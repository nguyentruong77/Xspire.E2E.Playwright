using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;
using Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.TaxCategories;

/// <summary>
/// Màn form New Tax Category (màn riêng, URL có GUID placeholder toàn 0).
/// </summary>
public class TaxCategoryNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public TaxCategoryNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    /// <summary>
    /// Ensure đã vào màn New: URL chứa TaxCategories/00000000-0000-0000-0000-000000000000 (locale-agnostic).
    /// </summary>
    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*TaxCategories/00000000-0000-0000-0000-000000000000.*"));
    }

    // Tab/breadcrumb "Tax Categories - New" (chỉ đúng khi dùng tiếng Anh)
    public ILocator TabNew => _page.GetByText("Tax Categories - New", new() { Exact = true });

    // Form fields – dùng name cố định (locale-agnostic), không dùng id vì id sinh GUID mỗi lần
    public ILocator InputCode => _page.Locator("input[name='ne_Txt_Code']");
    public ILocator InputDescription => _page.Locator("input[name='ne_Txt_Description']");

    // Label hiển thị lỗi Code (full XPath – dùng khi viewport cố định 1920×1080)
    public ILocator StringErrorCode =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[2]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[1]/div/div");

    // Label hiển thị lỗi Description (full XPath – dùng khi viewport cố định 1920×1080)
    public ILocator StringErrorDescription =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[2]/div[2]/div[1]/div/div/div/form/dxbl-form-layout/div/dxbl-form-layout-item[2]/div/div");

    // Nút Save - ưu tiên locator theo role/name thay vì XPath tuyệt đối
    public ILocator ButtonSave =>
        _page.GetByRole(AriaRole.Button, new() { Name = "Save" }).First;

    // Nút Back (dùng thay cho Cancel) - full XPath
    public ILocator ButtonBack =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[4]/div[1]/div[2]/div/div[1]/button");

    // ===== Support methods tương tự LoginPage (sử dụng TaxCategoriesTestData) =====
    public async Task FillValidDataAsync()
    {
        await InputCode.FillAsync(TaxCategoriesTestData.CreateValid.Code);
        await InputDescription.FillAsync(TaxCategoriesTestData.CreateValid.Description);
    }

    public async Task FillEmptyCodeAsync()
    {
        await InputCode.FillAsync(TaxCategoriesTestData.CreateMissingCode.Code);
        await InputDescription.FillAsync(TaxCategoriesTestData.CreateMissingCode.Description);
    }

    public async Task FillEmptyDescriptionAsync()
    {
        await InputCode.FillAsync(TaxCategoriesTestData.CreateMissingDescription.Code);
        await InputDescription.FillAsync(TaxCategoriesTestData.CreateMissingDescription.Description);
    }

    public async Task ClickSaveAsync()
    {
        await ButtonSave.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await ButtonSave.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickBackAsync()
    {
        await ButtonBack.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}
