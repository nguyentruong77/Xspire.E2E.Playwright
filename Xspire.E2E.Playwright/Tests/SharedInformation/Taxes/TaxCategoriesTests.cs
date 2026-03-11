using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Common;
using Xspire.E2E.Playwright.Pages.SharedInformation;
using Xspire.E2E.Playwright.Pages.SharedInformation.Taxes;

namespace Xspire.E2E.Playwright.Tests.SharedInformation.Taxes;

/// <summary>
/// Basic navigation smoke tests for Shared Information - Tax Categories screen.
/// </summary>
public class TaxCategoriesTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public TaxCategoriesTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Should_Navigate_To_Tax_Categories_From_Home_Menu()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var loginPage = new LoginPage(page, settings);
        var homePage = new HomePage(page, settings);
        var sharedInfoPage = new SharedInformationPage(page, settings);
        var taxCategoriesPage = new TaxCategoriesPage(page, settings);

        // Arrange: login and land on home
        await loginPage.EnsureLoginPageAsync();
        await loginPage.LoginAsync(settings.ValidUser, settings.ValidPassword);
        await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // Act: navigate to Shared Information, then Tax Categories
        await homePage.NavigateToSharedInformationAsync();
        await sharedInfoPage.NavigateToTaxCategoriesAsync();

        // Assert: we should be on the Tax Categories screen (URL contains TaxCategories)
        await taxCategoriesPage.EnsureOnTaxCategoriesPageAsync();
    }
}

