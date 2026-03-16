using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.TaxCategories;
using Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Taxes;

/// <summary>
/// All tests for Shared Information - Taxes - Tax Categories (list + new).
/// 1 class = 1 browser (TestBase), các test case trong class chạy nối tiếp.
/// </summary>
[Collection("E2ESuite")]
public class TaxCategoriesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public TaxCategoriesTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Đảm bảo đã login:
    /// - Nếu đang ở about:blank hoặc trang login thì thực hiện login.
    /// - Nếu đã ở trong app (Home, TaxCategories, ...) thì giữ nguyên.
    /// </summary>
    private async Task EnsureLoggedInAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var url = page.Url ?? string.Empty;
        var needLogin =
            string.IsNullOrWhiteSpace(url) ||
            url == "about:blank" ||
            url.Contains("Account/Login", System.StringComparison.OrdinalIgnoreCase);

        if (needLogin)
        {
            var loginPage = new LoginPage(page, settings);
            await loginPage.EnsureLoginPageAsync();
            await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    /// <summary>
    /// Đảm bảo đang đứng ở màn list Tax Categories (coi như "home nhỏ" của module).
    /// Nếu chưa thì điều hướng thẳng tới URL SharedInformation/TaxCategories và chờ list sẵn sàng.
    /// </summary>
    private async Task EnsureOnTaxCategoriesListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxCategoriesPage = new TaxCategoriesPage(page, settings);

        // Nếu chưa ở đúng URL TaxCategories (chỉ chấp nhận .../SharedInformation/TaxCategories hoặc .../SharedInformation/TaxCategories/ ) thì điều hướng lại
        if (!page.Url.EndsWith("/SharedInformation/TaxCategories", System.StringComparison.OrdinalIgnoreCase) &&
            !page.Url.EndsWith("/SharedInformation/TaxCategories/", System.StringComparison.OrdinalIgnoreCase))
        {
            var targetUrl = settings.BaseUrl.TrimEnd('/') + "/SharedInformation/TaxCategories";
            await page.GotoAsync(targetUrl);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }

        // Đảm bảo list đã sẵn sàng (URL đúng + nút New hiển thị)
        await taxCategoriesPage.EnsureOnTaxCategoriesPageAsync();
        await taxCategoriesPage.ButtonNew.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = settings.StandardTimeoutMs // list có thể load chậm hơn 5s
        });
    }

    /// <summary>
    /// Flow chuẩn để mở form New Tax Category:
    /// - Đảm bảo login
    /// - Đảm bảo đang ở list Tax Categories
    /// - Click New để vào form
    /// </summary>
    private async Task<TaxCategoryNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnTaxCategoriesListAsync();

        var taxCategoriesPage = new TaxCategoriesPage(page, settings);
        var newPage = new TaxCategoryNewPage(page, settings);

        await taxCategoriesPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    #region List

    [Fact]
    public async Task Should_Navigate_To_Tax_Categories_From_Home_Menu()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxCategoriesPage = new TaxCategoriesPage(page, settings);

        // Arrange: login (nếu cần) và về list Tax Categories
        await EnsureLoggedInAsync();
        await EnsureOnTaxCategoriesListAsync();

        // Assert: we should be on the Tax Categories screen (URL contains TaxCategories)
        await taxCategoriesPage.EnsureOnTaxCategoriesPageAsync();
    }

    #endregion

    #region New

    // [Fact]
    // public async Task TC_TAXCAT_NEW_001_Should_Show_Validation_Errors_For_Empty_Form()
    // {
    //     var page = _fixture.Page;
    //     var newPage = await NavigateToNewFormAsync();

    //     // Act: click Save with empty Code & Description
    //     await newPage.ClickSaveAsync();

    //     // Assert: both error labels visible (chờ tối đa StandardTimeoutMs cho validation render)
    //     var timeout = _fixture.Settings.StandardTimeoutMs;
    //     await Assertions.Expect(newPage.StringErrorCode).ToBeVisibleAsync(new() { Timeout = timeout });
    //     await Assertions.Expect(newPage.StringErrorDescription).ToBeVisibleAsync(new() { Timeout = timeout });
    // }

    // [Fact]
    // public async Task TC_TAXCAT_NEW_002_Should_Show_Validation_Error_For_Empty_Code()
    // {
    //     var newPage = await NavigateToNewFormAsync();

    //     // Arrange: only fill Description (sử dụng support method)
    //     await newPage.FillEmptyCodeAsync();

    //     // Act
    //     await newPage.ClickSaveAsync();

    //     // Assert: Code error visible (chờ tối đa StandardTimeoutMs cho validation render)
    //     await Assertions.Expect(newPage.StringErrorCode).ToBeVisibleAsync(new() { Timeout = _fixture.Settings.StandardTimeoutMs });
    // }

    // [Fact]
    // public async Task TC_TAXCAT_NEW_003_Should_Show_Validation_Error_For_Empty_Description()
    // {
    //     var newPage = await NavigateToNewFormAsync();

    //     // Arrange: only fill Code (sử dụng support method)
    //     await newPage.FillEmptyDescriptionAsync();

    //     // Act
    //     await newPage.ClickSaveAsync();

    //     // Assert: Description error visible (chờ tối đa StandardTimeoutMs cho validation render)
    //     await Assertions.Expect(newPage.StringErrorDescription).ToBeVisibleAsync(new() { Timeout = _fixture.Settings.StandardTimeoutMs });
    // }

    [Fact]
    public async Task TC_TAXCAT_NEW_004_Should_Create_Tax_Category_Successfully()
    {
        var page = _fixture.Page;
        var newPage = await NavigateToNewFormAsync();

        // Arrange: fill valid data (sử dụng support method)
        await newPage.FillValidDataAsync();

        // Act
        await newPage.ClickSaveAsync();

        // Assert (tạm thời đơn giản): quay lại màn list Tax Categories
        var listPage = new TaxCategoriesPage(page, _fixture.Settings);
        await listPage.EnsureOnTaxCategoriesPageAsync();
    }

    // [Fact]
    // public async Task TC_TAXCAT_NEW_005_Should_Show_Validation_Error_For_Duplicate_Code()
    // {
    //     var newPage = await NavigateToNewFormAsync();

    //     // Arrange: dùng Code trùng với CreateValid nhưng Description khác
    //     await newPage.FillValidDataAsync(); // tạo bản ghi đầu tiên
    //     await newPage.ClickSaveAsync();
    //     await EnsureOnTaxCategoriesListAsync();

    //     // Mở lại form New để tạo bản ghi trùng code
    //     newPage = await NavigateToNewFormAsync();
    //     await newPage.InputCode.FillAsync(TaxCategoriesTestData.CreateDuplicateCode.Code);
    //     await newPage.InputDescription.FillAsync(TaxCategoriesTestData.CreateDuplicateCode.Description);

    //     // Act
    //     await newPage.ClickSaveAsync();

    //     // Assert: lỗi duplicate thường hiện ở Code (chờ tối đa StandardTimeoutMs cho validation render)
    //     await Assertions.Expect(newPage.StringErrorCode).ToBeVisibleAsync(new() { Timeout = _fixture.Settings.StandardTimeoutMs });
    // }

    #endregion
}

