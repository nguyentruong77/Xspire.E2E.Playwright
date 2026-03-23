using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.GeoSalesStructure.TerritoryLevelDefinitions;
using Xspire.E2E.Playwright.TestData.SharedInformation.GeoSalesStructure;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.GeoSalesStructure;

/// <summary>
/// Shared Information &gt; Geo Sales Structure &gt; Territory Level Definitions — CRUD (Plan.md).
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class TerritoryLevelDefinitionsTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public TerritoryLevelDefinitionsTests(TestBase fixture)
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

        if (needLogin)
        {
            var loginPage = new LoginPage(page, settings);
            await loginPage.EnsureLoginPageAsync();
            await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }
    }

    private async Task<TerritoryLevelDefinitionsNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var homePage = new HomePage(page, settings);
        await homePage.NavigateToSharedInformationAsync();

        var sharedInformationPage = new SharedInformationPage(page, settings);
        await sharedInformationPage.NavigateToTerritoryLevelDefinitionsAsync();

        var listPage = new TerritoryLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();
        await listPage.OpenNewFormAsync();

        var newPage = new TerritoryLevelDefinitionsNewPage(page, settings);
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    #region C — Create

    [Fact]
    [TestPriority(1)]
    public async Task TC_TERRITORYLD_NEW_001_Should_Create_TerritoryLevelDefinition_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        var newPage = await NavigateToNewFormAsync();

        await newPage.FillRequiredFieldsAsync(
            TerritoryLevelDefinitionsTestData.CreateValid.Code,
            TerritoryLevelDefinitionsTestData.CreateValid.Description,
            TerritoryLevelDefinitionsTestData.CreateValid.MaxLevel,
            TerritoryLevelDefinitionsTestData.CreateValid.LevelDescription,
            isActive: true);

        await newPage.ClickSaveAsync();

        var listPage = new TerritoryLevelDefinitionsPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();
    }

    #endregion

    #region R — Read

    [Fact]
    [TestPriority(2)]
    public async Task TC_TERRITORYLD_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TerritoryLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
    }

    #endregion

    #region U — Update

    [Fact]
    [TestPriority(3)]
    public async Task TC_TERRITORYLD_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TerritoryLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);

        var editPage = new TerritoryLevelDefinitionsNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(TerritoryLevelDefinitionsTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(TerritoryLevelDefinitionsTestData.EditDescription.NewDescription);
    }

    #endregion

    #region D — Delete

    [Fact]
    [TestPriority(4)]
    public async Task TC_TERRITORYLD_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TerritoryLevelDefinitionsPage(page, settings);
        await listPage.EnsureOnTerritoryLevelDefinitionsListAsync();

        await listPage.FillSearchAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(TerritoryLevelDefinitionsTestData.SearchSuccess.Code);
    }

    #endregion
}
