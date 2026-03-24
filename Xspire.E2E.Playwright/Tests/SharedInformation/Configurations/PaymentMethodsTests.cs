using Microsoft.Playwright;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.SharedInformation.Configurations.PaymentMethods;
using Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Configurations;

/// <summary>
/// Shared Information &gt; Configurations &gt; Payment Methods - CRUD.
/// </summary>
[Collection("E2ESuite")]
[TestCaseOrderer(PriorityOrderer.TypeName, PriorityOrderer.AssemblyName)]
public class PaymentMethodsTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public PaymentMethodsTests(TestBase fixture)
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

    private async Task EnsureOnPaymentMethodsListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var paymentMethodsPage = new PaymentMethodsPage(page, settings);
        await paymentMethodsPage.EnsureOnPaymentMethodsListAsync();
    }

    private async Task<PaymentMethodsNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnPaymentMethodsListAsync();

        var listPage = new PaymentMethodsPage(page, settings);
        var newPage = new PaymentMethodsNewPage(page, settings);

        await listPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    [Fact]
    [TestPriority(0)]
    public async Task Should_Navigate_To_PaymentMethods_List_From_Url()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var paymentMethodsPage = new PaymentMethodsPage(page, settings);

        await EnsureLoggedInAsync();
        await EnsureOnPaymentMethodsListAsync();

        await paymentMethodsPage.EnsureOnPaymentMethodsPageAsync();
    }

    [Fact]
    [TestPriority(1)]
    public async Task TC_PAYM_NEW_001_Should_Create_Payment_Method_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillValidDataAsync();
        await newPage.ClickSaveAsync();

        var listPage = new PaymentMethodsPage(page, settings);
        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnPaymentMethodsListAsync();
    }

    [Fact]
    [TestPriority(2)]
    public async Task TC_PAYM_SEARCH_001_Should_Search_By_Payment_Method_Code_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new PaymentMethodsPage(page, settings);
        await listPage.EnsureOnPaymentMethodsListAsync();

        await listPage.FillSearchAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(PaymentMethodsTestData.SearchSuccess.Code);
    }

    [Fact]
    [TestPriority(3)]
    public async Task TC_PAYM_EDIT_001_Should_Edit_Description_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new PaymentMethodsPage(page, settings);
        await listPage.EnsureOnPaymentMethodsListAsync();

        await listPage.FillSearchAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(PaymentMethodsTestData.SearchSuccess.Code);

        var editPage = new PaymentMethodsNewPage(page, settings);
        await listPage.OpenEditFormByCodeAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await editPage.EnsureOnEditPageAsync(PaymentMethodsTestData.SearchSuccess.Code);

        await editPage.FillDescriptionOnlyAsync(PaymentMethodsTestData.EditDescription.NewDescription);
        await editPage.ClickSaveAsync();

        await listPage.EnsureSuccessOnDetailAsync();
        await listPage.EnsureOnPaymentMethodsListAsync();

        await listPage.FillSearchAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureDescriptionInGridAsync(PaymentMethodsTestData.EditDescription.NewDescription);
    }

    [Fact]
    [TestPriority(4)]
    public async Task TC_PAYM_DELETE_001_Should_Delete_Record_Successfully()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();

        var listPage = new PaymentMethodsPage(page, settings);
        await listPage.EnsureOnPaymentMethodsListAsync();

        await listPage.FillSearchAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureSearchSuccessAsync(PaymentMethodsTestData.SearchSuccess.Code);

        await listPage.OpenActionMenuForCodeAsync(PaymentMethodsTestData.SearchSuccess.Code);

        var deleteMenuItem = page.GetByText("Delete", new() { Exact = true });
        await deleteMenuItem.ClickAsync();

        var confirmYesButton = page.GetByRole(AriaRole.Button, new() { Name = "Yes" });
        await confirmYesButton.ClickAsync();

        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await listPage.FillSearchAsync(PaymentMethodsTestData.SearchSuccess.Code);
        await listPage.EnsureRecordDeletedAsync(PaymentMethodsTestData.SearchSuccess.Code);
    }
}
