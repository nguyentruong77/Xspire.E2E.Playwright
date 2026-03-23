using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.TaxCategories;
using Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Taxes;

/// <summary>
/// Shared Information &gt; Taxes &gt; Tax Categories — CRUD (cùng pattern Geography/Territory Level Definitions).
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class TaxCategoriesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public TaxCategoriesTests(TestBase fixture)
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

    private async Task EnsureOnTaxCategoriesListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxCategoriesPage = new TaxCategoriesPage(page, settings);
        await taxCategoriesPage.EnsureOnTaxCategoriesListAsync();
    }

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
    [TestPriority(0)]
    public async Task Should_Navigate_To_Tax_Categories_From_Home_Menu()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxCategoriesPage = new TaxCategoriesPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnTaxCategoriesListAsync();

        await taxCategoriesPage.EnsureOnTaxCategoriesPageAsync();
    }

    #endregion

    #region New

    [Fact]
    [TestPriority(1)]
    public async Task TC_TAXCAT_NEW_004_Should_Create_Tax_Category_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillValidDataAsync();

        await newPage.ClickSaveAsync();

        var listPage = new TaxCategoriesPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTaxCategoriesListAsync();
    }

    #endregion

    #region R — Read (search)

    [Fact]
    [TestPriority(2)]
    public async Task TC_TAXCAT_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxCategoriesPage(page, settings);
        await listPage.EnsureOnTaxCategoriesListAsync();

        await listPage.FillSearchAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxCategoriesTestData.SearchSuccess.Code);
    }

    #endregion

    #region U — Update

    [Fact]
    [TestPriority(3)]
    public async Task TC_TAXCAT_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxCategoriesPage(page, settings);
        await listPage.EnsureOnTaxCategoriesListAsync();

        await listPage.FillSearchAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxCategoriesTestData.SearchSuccess.Code);

        var editPage = new TaxCategoryNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(TaxCategoriesTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(TaxCategoriesTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTaxCategoriesListAsync();

        await listPage.FillSearchAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(TaxCategoriesTestData.EditDescription.NewDescription);
    }

    #endregion

    #region D — Delete

    [Fact]
    [TestPriority(4)]
    public async Task TC_TAXCAT_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxCategoriesPage(page, settings);
        await listPage.EnsureOnTaxCategoriesListAsync();

        await listPage.FillSearchAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxCategoriesTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(TaxCategoriesTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(TaxCategoriesTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(TaxCategoriesTestData.SearchSuccess.Code);
    }

    #endregion
}
