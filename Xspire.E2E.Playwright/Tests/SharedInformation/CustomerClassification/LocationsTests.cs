using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.CustomerClassification.Locations;
using Xspire.E2E.Playwright.TestData.SharedInformation.CustomerClassification;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.CustomerClassification;

/// <summary>
/// Tests for Shared Information &gt; Customer Classification &gt; Locations.
/// CRUD flow follows <c>GeographyLevelDefinitionsTests</c> pattern:
/// click <c>Code</c> to open edit, click <c>Description</c> to select row for delete.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class LocationsTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public LocationsTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    private async Task EnsureLoggedInAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var url = page.Url ?? string.Empty;
        var needLogin =
            string.IsNullOrWhiteSpace(url) ||
            url == "about:blank" ||
            url.Contains("Account/Login", System.StringComparison.OrdinalIgnoreCase);

        if (!needLogin)
            return;

        var loginPage = new LoginPage(page, settings);
        await loginPage.EnsureLoginPageAsync();
        await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    private async Task<LocationsNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var homePage = new HomePage(page, settings);
        await homePage.NavigateToSharedInformationAsync();

        var sharedInformationPage = new SharedInformationPage(page, settings);
        await sharedInformationPage.NavigateToLocationsAsync();

        var listPage = new LocationsPage(page, settings);
        await listPage.EnsureOnLocationsListAsync();
        await listPage.OpenNewFormAsync();

        var newPage = new LocationsNewPage(page, settings);
        await newPage.EnsureOnNewPageAsync();
        return newPage;
    }

    #region C — Create

    [Fact]
    [TestPriority(1)]
    public async Task TC_LOCATIONS_NEW_001_Should_Create_Location_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var newPage = await NavigateToNewFormAsync();

        await newPage.FillRequiredFieldsAsync(
            LocationsTestData.CreateValid.Code,
            LocationsTestData.CreateValid.Description);

        await newPage.ClickSaveAsync();

        var listPage = new LocationsPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.GotoLocationsListAsync();
    }

    #endregion

    #region R — Read (list / search)

    [Fact]
    [TestPriority(2)]
    public async Task TC_LOCATIONS_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new LocationsPage(page, settings);
        await listPage.EnsureOnLocationsListAsync();

        await listPage.FillSearchAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(LocationsTestData.SearchSuccess.Code);
    }

    #endregion

    #region U — Update

    [Fact]
    [TestPriority(3)]
    public async Task TC_LOCATIONS_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new LocationsPage(page, settings);
        await listPage.EnsureOnLocationsListAsync();

        await listPage.FillSearchAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(LocationsTestData.SearchSuccess.Code);

        // Open edit by clicking Code cell.
        var editPage = new LocationsNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(LocationsTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(LocationsTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(LocationsTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();

        // Always goto list after edit (fresh DOM/state for assertions/search).
        await listPage.GotoLocationsListAsync();
        await listPage.FillSearchAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(LocationsTestData.EditDescription.NewDescription);
    }

    #endregion

    #region D — Delete

    [Fact]
    [TestPriority(4)]
    public async Task TC_LOCATIONS_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new LocationsPage(page, settings);
        await listPage.EnsureOnLocationsListAsync();

        await listPage.FillSearchAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(LocationsTestData.SearchSuccess.Code);

        // Open action menu for selected row (old logic: click Description cell, not Code).
        await listPage.OpenActionMenuForCodeAsync(LocationsTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(LocationsTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(LocationsTestData.SearchSuccess.Code);
    }

    #endregion
}

