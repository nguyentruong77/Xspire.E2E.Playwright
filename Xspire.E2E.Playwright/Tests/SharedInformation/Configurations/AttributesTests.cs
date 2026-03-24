using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.Attributes;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Configurations;

/// <summary>
/// Shared Information &gt; Configurations &gt; Attributes - CRUD.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class AttributesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public AttributesTests(TestBase fixture)
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

    private async Task EnsureOnAttributesListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var attributesPage = new AttributesPage(page, settings);
        await attributesPage.EnsureOnAttributesListAsync();
    }

    private async Task<AttributeNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnAttributesListAsync();

        var attributesPage = new AttributesPage(page, settings);
        var newPage = new AttributeNewPage(page, settings);

        await attributesPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    [Fact]
    [TestPriority(0)]
    public async Task Should_Navigate_To_Attributes_List_From_Url()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var attributesPage = new AttributesPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnAttributesListAsync();

        await attributesPage.EnsureOnAttributesPageAsync();
    }

    [Fact]
    [TestPriority(1)]
    public async Task TC_ATTR_NEW_001_Should_Create_Attribute_With_Checkbox_Values_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.InputCode.FillAsync(AttributesTestData.CreateValid.Code);
        await newPage.SelectControlTypeCheckboxAsync();
        await newPage.InputDescription.FillAsync(AttributesTestData.CreateValid.Description);
        await newPage.FillCheckboxCreateValueRowsAsync();
        await newPage.ClickSaveAsync();

        var listPage = new AttributesPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnAttributesListAsync();
    }

    [Fact]
    [TestPriority(2)]
    public async Task TC_ATTR_SEARCH_001_Should_Search_By_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new AttributesPage(page, settings);
        await listPage.EnsureOnAttributesListAsync();

        await listPage.FillSearchAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(AttributesTestData.SearchSuccess.Code);
    }

    [Fact]
    [TestPriority(3)]
    public async Task TC_ATTR_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new AttributesPage(page, settings);
        await listPage.EnsureOnAttributesListAsync();

        await listPage.FillSearchAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(AttributesTestData.SearchSuccess.Code);

        var editPage = new AttributeNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(AttributesTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(AttributesTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(AttributesTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnAttributesListAsync();

        await listPage.FillSearchAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(AttributesTestData.EditDescription.NewDescription);
    }

    [Fact]
    [TestPriority(4)]
    public async Task TC_ATTR_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new AttributesPage(page, settings);
        await listPage.EnsureOnAttributesListAsync();

        await listPage.FillSearchAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(AttributesTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(AttributesTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(AttributesTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(AttributesTestData.SearchSuccess.Code);
    }
}
