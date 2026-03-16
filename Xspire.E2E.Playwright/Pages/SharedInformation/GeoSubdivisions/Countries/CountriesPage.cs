using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation.GeoSubdivisions.Countries;

/// <summary>
/// Represents the Countries screen under Shared Information &gt; Geo Subdivisions.
/// Skeleton structure similar to TaxCategoriesPage; locators can be refined later.
/// </summary>
public class CountriesPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public CountriesPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Page title/header - temporary text-based locator; you can adjust XPath/CSS later.
    public ILocator PageTitle =>
        _page.Locator("h1, h2").Filter(new LocatorFilterOptions { HasTextString = "Countries" });

    /// <summary>
    /// Simple assertion helper to ensure we are on the Countries page (list hoặc detail).
    /// </summary>
    public async Task EnsureOnCountriesPageAsync()
    {
        // Tạm thời assert theo URL chứa 'Countries'; bạn có thể chỉnh lại pattern nếu route khác.
        await Assertions.Expect(_page).ToHaveURLAsync(new System.Text.RegularExpressions.Regex(".*Countries.*"));
    }

    /// <summary>
    /// Đảm bảo đang ở màn list Countries (home của module).
    /// Luôn GotoAsync về /SharedInformation/Countries để F5 trang,
    /// xoá hết các tab/detail đang mở để không ảnh hưởng các testcase sau.
    /// </summary>
    public async Task EnsureOnCountriesListAsync()
    {
        var baseUrl = _settings.BaseUrl.TrimEnd('/');
        await _page.GotoAsync(baseUrl + "/SharedInformation/Countries");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        await EnsureOnCountriesPageAsync();
        await ButtonNew.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>
    /// Assert lưu thành công dựa trên URL detail:
    /// - New form luôn có GUID all-zero: .../Countries/00000000-0000-0000-0000-000000000000
    /// - Lưu thành công sẽ chuyển sang GUID thật: .../Countries/{real-guid}
    /// => Chỉ cần URL chứa /Countries/ và KHÔNG chứa GUID all-zero là coi như thành công.
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // URL phải là trang Countries detail bất kỳ (có GUID), không còn all-zero GUID của màn New.
        await Assertions.Expect(_page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(".*/SharedInformation/Countries/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    /// <summary>
    /// Từ màn detail (sau Save), bấm Back để về list Countries. Logic "home" của module = CountriesPage (list).
    /// </summary>
    public async Task NavigateBackToListAsync()
    {
        await ClickToolbarButtonAsync("Back");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await EnsureOnCountriesPageAsync();
    }

    // ===== List screen buttons – dùng list + filter để tái sử dụng cho nhiều tên nút =====

    /// <summary>
    /// Tập hợp các nút trên toolbar/list (New, Edit, ...).
    /// Selector tạm thời: mọi button.dxbl-btn; bạn có thể thu hẹp scope sau nếu cần.
    /// </summary>
    public ILocator ToolbarButtons =>
        _page.Locator("button.dxbl-btn");

    /// <summary>Lấy button theo text hiển thị (ví dụ \"New\", \"Edit\").</summary>
    public ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    /// <summary>Click button bất kỳ trên toolbar theo text (dễ tái sử dụng).</summary>
    public async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public ILocator ButtonNew => GetToolbarButton("New");

    public ILocator ButtonSearch => GetToolbarButton("Search");

    /// <summary>Ô search trên màn list (full XPath – có thể đổi sang selector ổn định hơn sau).</summary>
    public ILocator SearchInput =>
        _page.Locator("xpath=/html/body/div[3]/div/div[2]/div/div/div[2]/div[2]/div[2]/div/div[2]/div[1]/dxbl-input-editor/input");

    /// <summary>Gõ mã vào ô search (grid có thể filter khi Enter hoặc blur – tuỳ UI).</summary>
    public async Task FillSearchAsync(string code)
    {
        await SearchInput.FillAsync(code);
        await SearchInput.PressAsync("Enter");
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Ensure search thành công: (a) ô search có value đúng, (c) bảng có ít nhất một ô Code khớp CHÍNH XÁC mã mong đợi.
    /// Cột Code: td[data-caption=\"Code\"] có text = expectedCode (so sánh exact bằng regex ^code$ để tránh dính CPC1, CPC2,...).
    /// </summary>
    public async Task EnsureSearchSuccessAsync(string expectedCode)
    {
        var timeout = _settings.StandardTimeoutMs;
        await Assertions.Expect(SearchInput).ToHaveValueAsync(expectedCode, new() { Timeout = timeout });

        var exactCodeCell = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(expectedCode)}$")
        });

        await Assertions.Expect(exactCodeCell.First).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    /// <summary>
    /// Ensure cột Description trong grid chứa đúng mô tả mong đợi (sau khi search/ filter).
    /// So sánh theo thuộc tính title (full text), vì text hiển thị có thể bị rút gọn với dấu ...
    /// </summary>
    public async Task EnsureDescriptionInGridAsync(string expectedDescription)
    {
        var timeout = _settings.StandardTimeoutMs;
        var cellLink = _page.Locator($"td[data-caption='Description'] a[title='{expectedDescription}']");
        await Assertions.Expect(cellLink.First).ToBeVisibleAsync(new() { Timeout = timeout });
    }

    /// <summary>
    /// Ensure đã xoá xong một Country theo Code:
    /// - Sau khi search, không còn bất kỳ ô Code nào có text CHÍNH XÁC = code nữa.
    /// - Trường hợp không còn dòng nào (hiện "No Data Available") cũng thoả điều kiện này.
    /// </summary>
    public async Task EnsureCountryDeletedAsync(string code)
    {
        var timeout = _settings.StandardTimeoutMs;
        var exactCodeCells = _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(code)}$")
        });

        await Assertions.Expect(exactCodeCells).ToHaveCountAsync(0, new() { Timeout = timeout });
    }

    /// <summary>Opens the New Country form (modal hoặc màn hình riêng).</summary>
    public async Task OpenNewFormAsync()
    {
        await ClickToolbarButtonAsync("New");
    }

    /// <summary>
    /// Ô CODE (td) trong cột Code ứng với một mã cụ thể (ví dụ CPC) – dùng để mở form Edit.
    /// Bạn xác nhận click trực tiếp td này sẽ vào form edit. So sánh exact để tránh dính CPC1, CPC2,...
    /// </summary>
    public ILocator CodeCellByCode(string code) =>
        _page.Locator("td[data-caption='Code']").Filter(new LocatorFilterOptions
        {
            HasTextRegex = new System.Text.RegularExpressions.Regex($"^{System.Text.RegularExpressions.Regex.Escape(code)}$")
        });

    /// <summary>
    /// Mở form Edit bằng cách click vào ô CODE trong grid (ví dụ ô chứa chữ CPC).
    /// Giả định rằng grid đã được filter/hiển thị đúng dòng trước đó.
    /// </summary>
    public async Task OpenEditFormByCodeAsync(string code)
    {
        var cell = CodeCellByCode(code);
        await cell.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    /// <summary>
    /// Nút Action (mũi tên dropdown) ở góc phải grid Countries.
    /// Dùng class dxbl-btn-primary + dxbl-btn-split-dropdown để phân biệt với nút secondary bên cạnh.
    /// </summary>
    public ILocator ActionDropdownButton =>
        _page.Locator("button.dxbl-btn-primary.dxbl-btn-split-dropdown");

    /// <summary>
    /// Chọn dòng theo Code rồi mở menu Action (chuẩn bị cho thao tác Delete, ...).
    /// - Dùng Code để tìm đúng row (tránh dính CPC1, CPC2,...).
    /// - Trong row đó click vào ô Description (td[data-caption='Description']) để select dòng.
    /// - Sau đó click nút Action dropdown.
    /// </summary>
    public async Task OpenActionMenuForCodeAsync(string code)
    {
        // 1. Tìm cell Code chính xác theo mã
        var codeCell = CodeCellByCode(code).First;

        // 2. Lấy row chứa cell Code
        var row = codeCell.Locator("xpath=ancestor::tr[1]");

        // 3. Click vào ô Description trong cùng row để select dòng
        var descriptionCell = row.Locator("td[data-caption='Description']");
        await descriptionCell.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

        // 4. Mở menu Action (dropdown)
        await ActionDropdownButton.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
}

