using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit.Sdk;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.GeoSubdivisions.Countries;
using Xspire.E2E.Playwright.TestData.SharedInformation.GeoSubdivisions;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.GeoSubdivisions;

/// <summary>
/// Tests cho Shared Information &gt; Geo Subdivisions &gt; Countries.
/// Một class = một browser (<see cref="TestBase"/>); thứ tự chạy cố định theo CRUD nhờ <see cref="PriorityOrderer"/>.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class CountriesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public CountriesTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Đảm bảo đã login:
    /// - Nếu đang ở about:blank hoặc trang login thì thực hiện login.
    /// - Nếu đã ở trong app (Home, Shared Information, Countries, ...) thì giữ nguyên.
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
    /// Flow chuẩn để mở form New Country:
    /// - Đảm bảo login.
    /// - Từ Home &gt; Shared Information.
    /// - Từ Shared Information &gt; Countries.
    /// - Ở list Countries, click New để vào màn Countries - New.
    /// </summary>
    private async Task<CountriesNewPage> NavigateToNewCountryFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        // Từ Home đi tới Shared Information
        var homePage = new HomePage(page, settings);
        await homePage.NavigateToSharedInformationAsync();

        // Từ Shared Information đi tới Countries
        var sharedInformationPage = new SharedInformationPage(page, settings);
        await sharedInformationPage.NavigateToCountriesAsync();

        // Đảm bảo đang ở list Countries và mở form New
        var countriesPage = new CountriesPage(page, settings);
        await countriesPage.EnsureOnCountriesListAsync();
        await countriesPage.OpenNewFormAsync();

        var newPage = new CountriesNewPage(page, settings);
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    #region C — Create

    [Fact]
    [TestPriority(1)]
    public async Task TC_COUNTRY_NEW_001_Should_Create_Country_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var newPage = await NavigateToNewCountryFormAsync();

        // Arrange: fill required fields (Code, Time Zone, Description) từ TestData.
        await newPage.FillRequiredFieldsAsync(
            CountriesTestData.CreateValid.Code,
            CountriesTestData.CreateValid.TimeZoneText,
            CountriesTestData.CreateValid.Description);

        // Act
        await newPage.ClickSaveAsync();

        // Assert: URL detail đã chuyển sang GUID thật (không còn all-zero), rồi navigate về list Countries.
        var countriesPage = new CountriesPage(page, settings);
        await countriesPage.EnsureSuccessOnDetailAsync();
        await countriesPage.NavigateBackToListAsync();
    }

    #endregion

    #region R — Read (list / search)

    /// <summary>
    /// Đọc/lọc trên list: search theo mã sau khi đã tạo (Create). Phụ thuộc thứ tự CRUD.
    /// </summary>
    [Fact]
    [TestPriority(2)]
    public async Task TC_COUNTRY_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        // Arrange: đảm bảo đang ở list Countries.
        var countriesPage = new CountriesPage(page, settings);
        await EnsureLoggedInAsync();

        // Nếu test này chạy sau test New trong cùng session, browser đã ở Countries list rồi.
        // Nếu chạy độc lập, dùng EnsureOnCountriesListAsync() để tự điều hướng về list.
        await countriesPage.EnsureOnCountriesListAsync();

        // Act: gõ mã search từ TestData
        await countriesPage.FillSearchAsync(CountriesTestData.SearchSuccess.Code);

        // Assert: (a) ô search có value đúng, (c) bảng có ô chứa mã
        await countriesPage.EnsureSearchSuccessAsync(CountriesTestData.SearchSuccess.Code);
    }

    #endregion

    #region U — Update

    /// <summary>
    /// Cập nhật Country: search theo Code, mở Edit, sửa Description rồi lưu; assert trên grid.
    /// Chạy sau Create + Search.
    /// </summary>
    [Fact]
    [TestPriority(3)]
    public async Task TC_COUNTRY_EDIT_001_Should_Edit_Country_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        // Bắt đầu từ list Countries
        var countriesPage = new CountriesPage(page, settings);
        await countriesPage.EnsureOnCountriesListAsync();

        // Search bản ghi cần edit
        await countriesPage.FillSearchAsync(CountriesTestData.SearchSuccess.Code);
        await countriesPage.EnsureSearchSuccessAsync(CountriesTestData.SearchSuccess.Code);

        // Mở form Edit bằng cách click vào Code trong grid
        var newPage = new CountriesNewPage(page, settings);
        await countriesPage.OpenEditFormByCodeAsync(CountriesTestData.SearchSuccess.Code);

        await newPage.EnsureOnEditPageAsync(CountriesTestData.SearchSuccess.Code);

        // Sửa Description theo dữ liệu test (Description mới)
        var newDescription = CountriesTestData.EditDescription.NewDescription;
        await newPage.InputDescription.FillAsync(newDescription);

        // Lưu lại
        await newPage.ClickSaveAsync();

        // Đảm bảo đang ở detail với GUID thật.
        await countriesPage.EnsureSuccessOnDetailAsync();

        // Goto list (full navigation): đóng tab/detail ẩn trong shell, DOM sạch trước khi search —
        // tránh GetByRole(Textbox).First dính ô Code form edit (ne_Txt_Code).
        await countriesPage.EnsureOnCountriesListAsync();

        // Search lại và kiểm tra Description đã được cập nhật trong grid
        await countriesPage.FillSearchAsync(CountriesTestData.SearchSuccess.Code);
        await countriesPage.EnsureSearchSuccessAsync(CountriesTestData.SearchSuccess.Code);
        await countriesPage.EnsureDescriptionInGridAsync(newDescription);
    }

    #endregion

    #region D — Delete

    /// <summary>
    /// Xoá Country theo Code: list → search → chọn dòng → Action → Delete → Yes.
    /// Chạy cuối cùng trong CRUD.
    /// </summary>
    [Fact]
    [TestPriority(4)]
    public async Task TC_COUNTRY_DELETE_001_Should_Open_Action_Menu_For_Selected_Row()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var countriesPage = new CountriesPage(page, settings);
        await countriesPage.EnsureOnCountriesListAsync();

        // Search bản ghi cần xoá theo Code (CPC)
        await countriesPage.FillSearchAsync(CountriesTestData.SearchSuccess.Code);
        await countriesPage.EnsureSearchSuccessAsync(CountriesTestData.SearchSuccess.Code);

        // Chọn dòng theo Code và mở menu Action (dropdown)
        await countriesPage.OpenActionMenuForCodeAsync(CountriesTestData.SearchSuccess.Code);

        // Click nút Delete trong menu Action
        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        // Xác nhận popup "Are you sure you want to delete this record?"
        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Search lại và ensure mã CPC đã bị xoá (không còn row nào có Code = CPC).
        await countriesPage.FillSearchAsync(CountriesTestData.SearchSuccess.Code);
        await countriesPage.EnsureCountryDeletedAsync(CountriesTestData.SearchSuccess.Code);
    }

    #endregion
}
