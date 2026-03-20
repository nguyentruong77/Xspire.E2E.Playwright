using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xunit;
using Xspire.E2E.Playwright.Infrastructure;
using Xspire.E2E.Playwright.Pages.Auth;
using Xspire.E2E.Playwright.Pages.Inventory.UnitOfMeasures;
using Xspire.E2E.Playwright.TestData.Inventory.UnitOfMeasures;

namespace Xspire.E2E.Playwright.Tests.Inventory.UnitOfMeasures;

/// <summary>
/// Unit Of Measures validation suite (TC-UOM-001 .. TC-UOM-008).
/// 1 class = 1 browser (TestBase), các test case chạy nối tiếp.
/// </summary>
[Collection("E2ESuite")]
public class UnitOfMeasuresTests : IClassFixture<TestBase>
{
    private readonly TestBase _fixture;

    public UnitOfMeasuresTests(TestBase fixture)
    {
        _fixture = fixture;
    }

    // ===== Shared helpers =====

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

    /// <summary>Đang ở màn chi tiết đã lưu (GUID thật), không phải list / không phải New.</summary>
    private static bool IsUnitOfMeasuresDetailWithSavedRecord(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;
        const string zero = "00000000-0000-0000-0000-000000000000";
        var m = Regex.Match(url, @"/Inventory/UnitOfMeasures/([0-9a-fA-F-]{36})", RegexOptions.IgnoreCase);
        return m.Success && !string.Equals(m.Groups[1].Value, zero, System.StringComparison.OrdinalIgnoreCase);
    }

    private async Task EnsureOnUnitOfMeasuresListAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;
        var listPage = new UnitOfMeasuresListPage(page, settings);

        if (!page.Url.Contains("/Inventory/UnitOfMeasures", System.StringComparison.OrdinalIgnoreCase)
            || page.Url.Contains("00000000-0000-0000-0000-000000000000", System.StringComparison.OrdinalIgnoreCase)
            || IsUnitOfMeasuresDetailWithSavedRecord(page.Url))
        {
            await listPage.EnsureOnListAsync();
        }
        else
        {
            await listPage.EnsureOnUnitOfMeasuresPageAsync();
            await listPage.ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
            {
                State = WaitForSelectorState.Visible,
                Timeout = settings.StandardTimeoutMs
            });
        }
    }

    private async Task<UnitOfMeasuresNewPage> NavigateToNewFormAsync()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnUnitOfMeasuresListAsync();

        var listPage = new UnitOfMeasuresListPage(page, settings);
        var newPage = new UnitOfMeasuresNewPage(page, settings);

        await listPage.OpenNewFormAsync();
        await newPage.EnsureOnNewPageAsync();

        return newPage;
    }

    /// <summary>Chờ UI validation render trước khi assert #ne_Txt_FromUnit-error.</summary>
    private static Task DelayBeforeFromUnitErrorCheckAsync() => Task.Delay(2000);

    /// <summary>Chờ UI validation render trước khi assert #ne_Txt_ToUnit-error.</summary>
    private static Task DelayBeforeToUnitErrorCheckAsync() => Task.Delay(2000);

    // ===== Test cases =====

    [Fact]
    public async Task TC_UOM_001_Validate_Form_Visibility()
    {
        var newPage = await NavigateToNewFormAsync();

        await Assertions.Expect(newPage.InputFromUnit).ToBeVisibleAsync();
        await Assertions.Expect(newPage.InputToUnit).ToBeVisibleAsync();
        await Assertions.Expect(newPage.GetToolbarButton("Save").First).ToBeVisibleAsync();
    }

    [Fact]
    public async Task TC_UOM_002_Should_Show_Validation_Errors_For_Empty_Form()
    {
        var newPage = await NavigateToNewFormAsync();

        await newPage.ClickSaveAsync();

        await DelayBeforeFromUnitErrorCheckAsync();
        var timeout = _fixture.Settings.StandardTimeoutMs;
        await Assertions.Expect(newPage.StringErrorFromUnit).ToBeVisibleAsync(new() { Timeout = timeout });
        await DelayBeforeToUnitErrorCheckAsync();
        await Assertions.Expect(newPage.StringErrorToUnit).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    [Fact]
    public async Task TC_UOM_003_Should_Show_Validation_Error_For_Empty_FromUnit()
    {
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillToUnitOnlyAsync(UnitOfMeasuresTestData.RequiredValidation.ToUnit);
        await newPage.ClickSaveAsync();

        await DelayBeforeFromUnitErrorCheckAsync();
        var timeout = _fixture.Settings.StandardTimeoutMs;
        await Assertions.Expect(newPage.StringErrorFromUnit).ToBeVisibleAsync(new() { Timeout = timeout });
        await DelayBeforeToUnitErrorCheckAsync();
        await Assertions.Expect(newPage.StringErrorToUnit).ToBeHiddenAsync();
    }

    [Fact]
    public async Task TC_UOM_004_Should_Show_Validation_Error_For_Empty_ToUnit()
    {
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillFromUnitOnlyAsync(UnitOfMeasuresTestData.RequiredValidation.FromUnit);
        await newPage.ClickSaveAsync();

        await DelayBeforeFromUnitErrorCheckAsync();
        var timeout = _fixture.Settings.StandardTimeoutMs;
        await Assertions.Expect(newPage.StringErrorFromUnit).ToBeHiddenAsync();
        await DelayBeforeToUnitErrorCheckAsync();
        await Assertions.Expect(newPage.StringErrorToUnit).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    [Fact]
    public async Task TC_UOM_005_Should_Save_With_Valid_Data()
    {
        var page = _fixture.Page;
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillUnitsAsync(
            UnitOfMeasuresTestData.CreateSuccess.FromUnit,
            UnitOfMeasuresTestData.CreateSuccess.ToUnit);
        await newPage.ClickSaveAsync();

        await DelayBeforeFromUnitErrorCheckAsync();
        var timeout = _fixture.Settings.StandardTimeoutMs;
        await Assertions.Expect(newPage.StringErrorFromUnit).ToBeHiddenAsync();
        await DelayBeforeToUnitErrorCheckAsync();
        await Assertions.Expect(newPage.StringErrorToUnit).ToBeHiddenAsync();

        await newPage.EnsureSuccessOnDetailAsync();
        await Assertions.Expect(newPage.InputFromUnit).ToHaveValueAsync(UnitOfMeasuresTestData.CreateSuccess.FromUnit);
    }

    [Fact]
    public async Task TC_UOM_006_Should_Show_Duplicate_Error_When_FromUnit_Exists()
    {
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillUnitsAsync(
            UnitOfMeasuresTestData.DuplicateScenario.FromUnit,
            UnitOfMeasuresTestData.DuplicateScenario.ToUnit);
        await newPage.ClickSaveAsync();

        await newPage.FromUnitDuplicateError.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _fixture.Settings.BigTimeoutMs
        });

        await Assertions.Expect(newPage.FromUnitDuplicateError).ToBeVisibleAsync();
    }

    [Fact]
    public async Task TC_UOM_007_Should_Show_Both_Errors_When_FromUnit_Duplicate_And_ToUnit_Empty()
    {
        var newPage = await NavigateToNewFormAsync();

        await newPage.FillFromUnitOnlyAsync(UnitOfMeasuresTestData.DuplicateScenario.FromUnit);
        await newPage.ClickSaveAsync();

        await DelayBeforeFromUnitErrorCheckAsync();
        var timeout = _fixture.Settings.StandardTimeoutMs;
        await Assertions.Expect(newPage.StringErrorFromUnit).ToBeVisibleAsync(new() { Timeout = timeout });
        await DelayBeforeToUnitErrorCheckAsync();
        await Assertions.Expect(newPage.StringErrorToUnit).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    [Fact(Skip = "Ô tìm kiếm grid chưa tương tác được trong E2E (input không visible). Bật lại khi locator/UX ổn định.")]
    public async Task TC_UOM_008_Should_Find_Unit_By_Name_In_Search()
    {
        var page = _fixture.Page;
        var settings = _fixture.Settings;

        await EnsureLoggedInAsync();
        await EnsureOnUnitOfMeasuresListAsync();

        var listPage = new UnitOfMeasuresListPage(page, settings);
        await listPage.SearchByUnitNameAsync(UnitOfMeasuresTestData.DuplicateScenario.FromUnit);
        await listPage.EnsureUnitNameInGridAsync(UnitOfMeasuresTestData.DuplicateScenario.FromUnit);
    }
}
