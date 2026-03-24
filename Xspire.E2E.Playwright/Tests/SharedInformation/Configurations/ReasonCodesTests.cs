using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.ReasonCodes;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Configurations;

/// <summary>
/// Shared Information > Configurations > Reason Codes - CRUD.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class ReasonCodesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public ReasonCodesTests(TestBase fixture)
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

    private async Task EnsureOnReasonCodesListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var listPage = new ReasonCodesPage(page, settings);
        await listPage.EnsureOnReasonCodesListAsync();
    }

    private async Task<ReasonCodesNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnReasonCodesListAsync();

        var listPage = new ReasonCodesPage(page, settings);
        var newPage = new ReasonCodesNewPage(page, settings);

        await listPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    [Fact]
    [TestPriority(0)]
    public async Task Should_Navigate_To_ReasonCodes_List_From_Url()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var listPage = new ReasonCodesPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnReasonCodesListAsync();

        await listPage.EnsureOnReasonCodesPageAsync();
    }

    [Fact]
    [TestPriority(1)]
    public async Task TC_RSNC_NEW_001_Should_Create_Reason_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillValidDataAsync();
        await newPage.ClickSaveAsync();

        var listPage = new ReasonCodesPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnReasonCodesListAsync();
    }

    [Fact]
    [TestPriority(2)]
    public async Task TC_RSNC_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new ReasonCodesPage(page, settings);
        await listPage.EnsureOnReasonCodesListAsync();

        await listPage.FillSearchAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(ReasonCodesTestData.SearchSuccess.Code);
    }

    [Fact]
    [TestPriority(3)]
    public async Task TC_RSNC_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new ReasonCodesPage(page, settings);
        await listPage.EnsureOnReasonCodesListAsync();

        await listPage.FillSearchAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(ReasonCodesTestData.SearchSuccess.Code);

        var editPage = new ReasonCodesNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(ReasonCodesTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(ReasonCodesTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(ReasonCodesTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnReasonCodesListAsync();

        await listPage.FillSearchAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(ReasonCodesTestData.EditDescription.NewDescription);
    }

    [Fact]
    [TestPriority(4)]
    public async Task TC_RSNC_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new ReasonCodesPage(page, settings);
        await listPage.EnsureOnReasonCodesListAsync();

        await listPage.FillSearchAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(ReasonCodesTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(ReasonCodesTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(ReasonCodesTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(ReasonCodesTestData.SearchSuccess.Code);
    }
}
