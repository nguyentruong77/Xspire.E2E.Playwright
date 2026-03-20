using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.GeoSubdivisions.GeographyLevelDefinitions;
using Xspire.E2E.Playwright.TestData.SharedInformation.GeoSubdivisions;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.GeoSubdivisions;

/// <summary>
/// Tests cho Shared Information &gt; Geo Subdivisions &gt; Geography Level Definitions.
/// Một class = một browser (<see cref="TestBase"/>); thứ tự chạy cố định theo CRUD nhờ <see cref="PriorityOrderer"/>.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class GeographyLevelDefinitionsTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public GeographyLevelDefinitionsTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    /// <summary>
    /// Đảm bảo đã login:
    /// - Nếu đang ở about:blank hoặc trang login thì thực hiện login.
    /// - Nếu đã ở trong app thì giữ nguyên.
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

    private async Task<GeographyLevelDefinitionsNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        // Home -> Shared Information -> Geography Level Definitions
        var homePage = new HomePage(page, settings);
        await homePage.NavigateToSharedInformationAsync();

        var sharedInformationPage = new SharedInformationPage(page, settings);
        await sharedInformationPage.NavigateToGeographyLevelDefinitionsAsync();

        var listPage = new GeographyLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnGeographyLevelDefinitionsListAsync();
        await listPage.OpenNewFormAsync();

        var newPage = new GeographyLevelDefinitionsNewPage(page, settings);
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    #region C — Create

    [Fact]
    [TestPriority(1)]
    public async Task TC_GEOGRAPHYLD_NEW_001_Should_Create_GeographyLevelDefinition_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var newPage = await NavigateToNewFormAsync();

        await newPage.FillRequiredFieldsAsync(
            GeographyLevelDefinitionsTestData.CreateValid.Code,
            GeographyLevelDefinitionsTestData.CreateValid.Description,
            GeographyLevelDefinitionsTestData.CreateValid.MaxLevel,
            GeographyLevelDefinitionsTestData.CreateValid.LevelDescription,
            isActive: true);

        await newPage.ClickSaveAsync();

        var listPage = new GeographyLevelDefinitionsPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.NavigateBackToListAsync();
    }

    #endregion

    #region R — Read (list / search)

    [Fact]
    [TestPriority(2)]
    public async Task TC_GEOGRAPHYLD_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new GeographyLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnGeographyLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
    }

    #endregion

    #region U — Update

    [Fact]
    [TestPriority(3)]
    public async Task TC_GEOGRAPHYLD_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new GeographyLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnGeographyLevelDefinitionsListAsync();

        // Search bản ghi cần edit
        await listPage.FillSearchAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);

        // Open edit by click Code cell
        var editPage = new GeographyLevelDefinitionsNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);

        // Update Description only (as requested)
        await editPage.FillDescriptionOnlyAsync(GeographyLevelDefinitionsTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();

        // Reset navigation state: go back to list
        await listPage.EnsureOnGeographyLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(GeographyLevelDefinitionsTestData.EditDescription.NewDescription);
    }

    #endregion

    #region D — Delete

    [Fact]
    [TestPriority(4)]
    public async Task TC_GEOGRAPHYLD_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new GeographyLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnGeographyLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);

        // Open Action menu and delete
        await listPage.OpenActionMenuForCodeAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(GeographyLevelDefinitionsTestData.SearchSuccess.Code);
    }

    #endregion
}

