using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Taxes.Taxes;
using Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Taxes;

/// <summary>
/// Shared Information > Taxes > Taxes - CRUD.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class TaxesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public TaxesTests(TestBase fixture)
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

    private async Task EnsureOnTaxesListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxesPage = new TaxesPage(page, settings);
        await taxesPage.EnsureOnTaxesListAsync();
    }

    private async Task<TaxNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnTaxesListAsync();

        var taxesPage = new TaxesPage(page, settings);
        var newPage = new TaxNewPage(page, settings);

        await taxesPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    [Fact]
    [TestPriority(0)]
    public async Task Should_Navigate_To_Taxes_From_Home_Menu()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var taxesPage = new TaxesPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnTaxesListAsync();

        await taxesPage.EnsureOnTaxesPageAsync();
    }

    [Fact]
    [TestPriority(1)]
    public async Task TC_TAX_NEW_001_Should_Create_Tax_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillValidDataAsync();
        await newPage.ClickSaveAsync();

        var listPage = new TaxesPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTaxesListAsync();
    }

    [Fact]
    [TestPriority(2)]
    public async Task TC_TAX_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxesPage(page, settings);
        await listPage.EnsureOnTaxesListAsync();

        await listPage.FillSearchAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxesTestData.SearchSuccess.Code);
    }

    [Fact]
    [TestPriority(3)]
    public async Task TC_TAX_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxesPage(page, settings);
        await listPage.EnsureOnTaxesListAsync();

        await listPage.FillSearchAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxesTestData.SearchSuccess.Code);

        var editPage = new TaxNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(TaxesTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(TaxesTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(TaxesTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnTaxesListAsync();

        await listPage.FillSearchAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(TaxesTestData.EditDescription.NewDescription);
    }

    [Fact]
    [TestPriority(4)]
    public async Task TC_TAX_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new TaxesPage(page, settings);
        await listPage.EnsureOnTaxesListAsync();

        await listPage.FillSearchAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(TaxesTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(TaxesTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(TaxesTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(TaxesTestData.SearchSuccess.Code);
    }
}
