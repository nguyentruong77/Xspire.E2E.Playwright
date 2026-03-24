using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.CreditTerms;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Configurations;

/// <summary>
/// Shared Information &gt; Configurations &gt; Credit Terms - CRUD.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class CreditTermsTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public CreditTermsTests(TestBase fixture)
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

    private async Task EnsureOnCreditTermsListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var creditTermsPage = new CreditTermsPage(page, settings);
        await creditTermsPage.EnsureOnCreditTermsListAsync();
    }

    private async Task<CreditTermsNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnCreditTermsListAsync();

        var creditTermsPage = new CreditTermsPage(page, settings);
        var newPage = new CreditTermsNewPage(page, settings);

        await creditTermsPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    [Fact]
    [TestPriority(0)]
    public async Task Should_Navigate_To_CreditTerms_List_From_Url()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var creditTermsPage = new CreditTermsPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnCreditTermsListAsync();

        await creditTermsPage.EnsureOnCreditTermsPageAsync();
    }

    [Fact]
    [TestPriority(1)]
    public async Task TC_CRTERM_NEW_001_Should_Create_Credit_Term_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillValidDataAsync();
        await newPage.ClickSaveAsync();

        var listPage = new CreditTermsPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnCreditTermsListAsync();
    }

    [Fact]
    [TestPriority(2)]
    public async Task TC_CRTERM_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new CreditTermsPage(page, settings);
        await listPage.EnsureOnCreditTermsListAsync();

        await listPage.FillSearchAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(CreditTermsTestData.SearchSuccess.Code);
    }

    [Fact]
    [TestPriority(3)]
    public async Task TC_CRTERM_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new CreditTermsPage(page, settings);
        await listPage.EnsureOnCreditTermsListAsync();

        await listPage.FillSearchAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(CreditTermsTestData.SearchSuccess.Code);

        var editPage = new CreditTermsNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(CreditTermsTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(CreditTermsTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(CreditTermsTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnCreditTermsListAsync();

        await listPage.FillSearchAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(CreditTermsTestData.EditDescription.NewDescription);
    }

    [Fact]
    [TestPriority(4)]
    public async Task TC_CRTERM_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new CreditTermsPage(page, settings);
        await listPage.EnsureOnCreditTermsListAsync();

        await listPage.FillSearchAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(CreditTermsTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(CreditTermsTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(CreditTermsTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(CreditTermsTestData.SearchSuccess.Code);
    }
}
