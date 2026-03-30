using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.Inventory.Warehouse;

/// <summary>
/// Màn form New Warehouse (URL chứa GUID toàn 0).
/// Cấu trúc tương tự UnitOfMeasuresNewPage / CountriesNewPage.
/// </summary>
public class WarehouseNewPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public WarehouseNewPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // ===== Navigation assertions =====

    public async Task EnsureOnNewPageAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*Warehouses/00000000-0000-0000-0000-000000000000.*"));
    }

    /// <summary>
    /// Assert lưu thành công: URL chuyển sang GUID thật (không còn all-zero).
    /// </summary>
    public async Task EnsureSuccessOnDetailAsync()
    {
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        await Assertions.Expect(_page).ToHaveURLAsync(
            new Regex(".*/Inventory/Warehouses/(?!00000000-0000-0000-0000-000000000000).+"),
            new() { Timeout = _settings.StandardTimeoutMs });
    }

    // ===== Form fields (ne_Txt_* / ne_Chk_* — ổn định theo name, tránh id GUID) =====

    /// <summary>Textbox Code — name/parent-id cố định; id class dxbl-text-edit-input có thể đổi mỗi render.</summary>
    public ILocator InputCode => _page.Locator("input[name='ne_Txt_Code']").Filter(new LocatorFilterOptions { Visible = true });

    public ILocator InputDescription => _page.Locator("input[name='ne_Txt_Description']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Account Name — name/parent-id cố định; id class có thể đổi mỗi render.</summary>
    public ILocator InputAccountName => _page.Locator("input[name='ne_Txt_AccountName']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Email — name/parent-id cố định.</summary>
    public ILocator InputEmail => _page.Locator("input[name='ne_Txt_Email']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Web — name/parent-id cố định.</summary>
    public ILocator InputWeb => _page.Locator("input[name='ne_Txt_Web']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Phone1 — name/parent-id cố định.</summary>
    public ILocator InputPhone1 => _page.Locator("input[name='ne_Txt_Phone1']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Phone2 — name/parent-id cố định.</summary>
    public ILocator InputPhone2 => _page.Locator("input[name='ne_Txt_Phone2']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Fax — name/parent-id cố định.</summary>
    public ILocator InputFax => _page.Locator("input[name='ne_Txt_Fax']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Attention — name/parent-id cố định.</summary>
    public ILocator InputAttention => _page.Locator("input[name='ne_Txt_Attention']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Address Line 1 — name/parent-id cố định.</summary>
    public ILocator InputAddressLine1 => _page.Locator("input[name='ne_Txt_AddressLine1']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Address Line 2 — name/parent-id cố định.</summary>
    public ILocator InputAddressLine2 => _page.Locator("input[name='ne_Txt_AddressLine2']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Postal Code — name/parent-id cố định.</summary>
    public ILocator InputPostalCode => _page.Locator("input[name='ne_Txt_PostalCode']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Textbox Carrier Facility — name/parent-id cố định.</summary>
    public ILocator InputCarrierFacility => _page.Locator("input[name='ne_Txt_CarrierFacility']").Filter(new LocatorFilterOptions { Visible = true });

    /// <summary>Memo Geography (read-only path) — <c>dxbl-memo-editor id="ne_Memo_GeographyPath"</c>.</summary>
    public ILocator MemoGeographyPath => _page.Locator("textarea[name='ne_Memo_GeographyPath']");

    /// <summary>Memo Territory (read-only display) — <c>dxbl-memo-editor id="ne_Memo_TerritoryDisplay"</c>.</summary>
    public ILocator MemoTerritoryDisplay => _page.Locator("textarea[name='ne_Memo_TerritoryDisplay']");

    /// <summary>Nút icon bên phải ô Email (envelope) trong <c>dxbl-input-editor#ne_Txt_Email</c>.</summary>
    public ILocator EmailTextEditActionButton =>
        _page.Locator("dxbl-input-editor#ne_Txt_Email").Locator("button.dxbl-text-edit-btn");

    /// <summary>Nút icon bên phải ô Web (external link) trong <c>dxbl-input-editor#ne_Txt_Web</c>.</summary>
    public ILocator WebTextEditActionButton =>
        _page.Locator("dxbl-input-editor#ne_Txt_Web").Locator("button.dxbl-text-edit-btn");

    public ILocator CheckboxActive => _page.Locator("input[type='checkbox'][name='ne_Chk_Active']");

    public ILocator CheckboxIsPublic => _page.Locator("input[type='checkbox'][name='ne_Chk_IsPublic']");

    // ===== DevExpress checkbox display view (dxbl-checkbox) =====
    // DevExpress thường render wrapper `div.dxbl-checkbox[role='checkbox']` với `aria-disabled`/`aria-checked`.
    // Dùng các locator này để assert trạng thái (disabled/unchecked/checked) nhanh và ổn định.
    private ILocator CheckboxActiveDisplay =>
        _page.Locator("div.dxbl-checkbox[role='checkbox']").Filter(new LocatorFilterOptions { Has = CheckboxActive });

    private ILocator CheckboxIsPublicDisplay =>
        _page.Locator("div.dxbl-checkbox[role='checkbox']").Filter(new LocatorFilterOptions { Has = CheckboxIsPublic });

    public ILocator CheckboxActiveDisabled =>
        CheckboxActiveDisplay.Locator("[aria-disabled='true']");

    public ILocator CheckboxActiveUnchecked =>
        CheckboxActiveDisplay.Locator("[aria-checked='false']");

    public ILocator CheckboxActiveDisabledUnchecked =>
        CheckboxActiveDisplay.Locator("[aria-disabled='true'][aria-checked='false']");

    public ILocator CheckboxIsPublicDisabled =>
        CheckboxIsPublicDisplay.Locator("[aria-disabled='true']");

    public ILocator CheckboxIsPublicUnchecked =>
        CheckboxIsPublicDisplay.Locator("[aria-checked='false']");

    public ILocator CheckboxIsPublicDisabledUnchecked =>
        CheckboxIsPublicDisplay.Locator("[aria-disabled='true'][aria-checked='false']");

    /// <summary>
    /// Host <c>dxbl-combo-box</c> bọc input <c>name="ne_Cbx_WarehouseType"</c> (ổn định hơn <c>#ne_Cbx_*</c> khi id đổi).
    /// </summary>
    private ILocator WarehouseTypeComboHost =>
        _page.Locator("dxbl-combo-box").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name='ne_Cbx_WarehouseType']")
        });

    public ILocator WarehouseTypeComboInput =>
        WarehouseTypeComboHost.Locator("input[role='combobox']");

    public ILocator WarehouseTypeDropdownButton =>
        WarehouseTypeComboHost.Locator(".dxbl-btn-group-right button");

    /// <summary>
    /// Combobox Vị trí nhận hàng (Receiving Location) - input name có suffix <c>_ne_Cbx_ReceivingLocation</c>.
    /// </summary>
    private ILocator ReceivingLocationComboHost =>
        _page.Locator("dxbl-combo-box").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name$='_ne_Cbx_ReceivingLocation']")
        });

    public ILocator ReceivingLocationComboInput =>
        ReceivingLocationComboHost.Locator("input[role='combobox']");

    public ILocator ReceivingLocationDropdownButton =>
        ReceivingLocationComboHost.Locator(".dxbl-btn-group-right button");

    /// <summary>
    /// Combobox Vị trí RMA (RMALocation) - input name có suffix <c>_ne_Cbx_RMALocation</c>.
    /// </summary>
    private ILocator RmaLocationComboHost =>
        _page.Locator("dxbl-combo-box").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name$='_ne_Cbx_RMALocation']")
        });

    public ILocator RmaLocationComboInput =>
        RmaLocationComboHost.Locator("input[role='combobox']");

    public ILocator RmaLocationDropdownButton =>
        RmaLocationComboHost.Locator(".dxbl-btn-group-right button");

    /// <summary>
    /// Combobox Vị trí giao hàng (Shipping Location) - input name có suffix <c>_ne_Cbx_ShippingLocation</c>.
    /// </summary>
    private ILocator ShippingLocationComboHost =>
        _page.Locator("dxbl-combo-box").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name$='_ne_Cbx_ShippingLocation']")
        });

    public ILocator ShippingLocationComboInput =>
        ShippingLocationComboHost.Locator("input[role='combobox']");

    public ILocator ShippingLocationDropdownButton =>
        ShippingLocationComboHost.Locator(".dxbl-btn-group-right button");

    /// <summary>
    /// Combobox DropShip Location - input name có suffix <c>_ne_Cbx_DropShipLocation</c>.
    /// </summary>
    private ILocator DropShipLocationComboHost =>
        _page.Locator("dxbl-combo-box").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name$='_ne_Cbx_DropShipLocation']")
        });

    public ILocator DropShipLocationComboInput =>
        DropShipLocationComboHost.Locator("input[role='combobox']");

    public ILocator DropShipLocationDropdownButton =>
        DropShipLocationComboHost.Locator(".dxbl-btn-group-right button");

    // ===== Validation errors =====

    private ILocator CodeFieldWrapper =>
        _page.Locator("div.dxbl-fl-ctrl").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name='ne_Txt_Code']")
        });

    private ILocator DescriptionFieldWrapper =>
        _page.Locator("div.dxbl-fl-ctrl").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("input[name='ne_Txt_Description']")
        });

    public ILocator StringErrorCode =>
        CodeFieldWrapper.Locator("div.validation-message");

    public ILocator StringErrorDescription =>
        DescriptionFieldWrapper.Locator("div.validation-message");

    public ILocator CodeDuplicateError =>
        CodeFieldWrapper.Locator(".dxbl-edit-validation-status");

    /// <summary>
    /// Tooltip validation (DevExpress) của textbox Code.
    /// Đoạn HTML bạn gửi có nội dung message nằm trong thuộc tính `title` của `.dxbl-edit-validation-status`.
    /// </summary>
    public ILocator CodeValidationTooltip =>
        CodeFieldWrapper.Locator(".dxbl-edit-validation-status").First;

    /// <summary>
    /// Icon invalid (dx-error-circle) tương ứng với `.dxbl-edit-validation-status-icon-invalid` trong snippet.
    /// </summary>
    public ILocator CodeValidationInvalidIcon =>
        CodeValidationTooltip.Locator(".dxbl-edit-validation-status-icon-invalid");

    public async Task<string> GetCodeValidationTitleAsync()
    {
        var title = await CodeValidationTooltip.First.GetAttributeAsync("title");
        return title ?? string.Empty;
    }

    /// <summary>
    /// Tooltip validation (DevExpress) của textbox Description.
    /// </summary>
    public ILocator DescriptionValidationTooltip =>
        DescriptionFieldWrapper.Locator(".dxbl-edit-validation-status").First;

    /// <summary>
    /// Icon invalid (dx-error-circle) tương ứng với `.dxbl-edit-validation-status-icon-invalid`.
    /// </summary>
    public ILocator DescriptionValidationInvalidIcon =>
        DescriptionValidationTooltip.Locator(".dxbl-edit-validation-status-icon-invalid");

    public async Task<string> GetDescriptionValidationTitleAsync()
    {
        var title = await DescriptionValidationTooltip.First.GetAttributeAsync("title");
        return title ?? string.Empty;
    }

    // ===== Toolbar =====

    public ILocator ToolbarButtons => _page.Locator("button.dxbl-btn");

    public ILocator GetToolbarButton(string text) =>
        ToolbarButtons.Filter(new LocatorFilterOptions { HasTextString = text });

    /// <summary>
    /// Nút primary Lưu (<c>span.dxbl-btn-caption</c>); fallback <c>Save</c> nếu đổi locale.
    /// </summary>
    public ILocator ButtonSave =>
        GetToolbarButton("Lưu").Or(GetToolbarButton("Save"));

    public async Task ClickToolbarButtonAsync(string text)
    {
        var button = GetToolbarButton(text);
        await button.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await button.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task ClickSaveAsync()
    {
        await ButtonSave.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await ButtonSave.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public Task ClickBackAsync() => ClickToolbarButtonAsync("Back");

    /// <summary>
    /// Popup button Geography (DevExpress standalone button).
    /// </summary>
    public ILocator GeographyPopupButton =>
        _page.Locator("button#ne_Btn_GeographyPopup[type='button']");

    /// <summary>
    /// Popup button Territory (DevExpress standalone button).
    /// </summary>
    public ILocator TerritoryPopupButton =>
        _page.Locator("button#ne_Btn_TerritoryPopup[type='button']");

    /// <summary>Nút Territory popup khi <c>disabled</c> (trước khi chọn Geography, v.v.).</summary>
    public ILocator TerritoryPopupButtonDisabled =>
        _page.Locator("button#ne_Btn_TerritoryPopup[disabled]");

    /// <summary>
    /// Chọn giá trị trong combobox Loại kho (mở dropdown, filter, chọn option theo text hiển thị).
    /// </summary>
    public async Task SelectWarehouseTypeAsync(string displayText)
    {
        await WarehouseTypeDropdownButton.First.ClickAsync();

        await WarehouseTypeComboInput.First.ClickAsync();
        await WarehouseTypeComboInput.First.FillAsync(displayText);

        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    /// <summary>
    /// Chọn giá trị trong combobox Vị trí nhận hàng (Receiving Location).
    /// </summary>
    public async Task SelectReceivingLocationAsync(string displayText)
    {
        await ReceivingLocationDropdownButton.First.ClickAsync();

        await ReceivingLocationComboInput.First.ClickAsync();
        await ReceivingLocationComboInput.First.FillAsync(displayText);

        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    /// <summary>
    /// Chọn giá trị trong combobox Vị trí RMA (RMALocation).
    /// </summary>
    public async Task SelectRmaLocationAsync(string displayText)
    {
        await RmaLocationDropdownButton.First.ClickAsync();

        await RmaLocationComboInput.First.ClickAsync();
        await RmaLocationComboInput.First.FillAsync(displayText);

        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    /// <summary>
    /// Chọn giá trị trong combobox Vị trí giao hàng (Shipping Location).
    /// </summary>
    public async Task SelectShippingLocationAsync(string displayText)
    {
        await ShippingLocationDropdownButton.First.ClickAsync();

        await ShippingLocationComboInput.First.ClickAsync();
        await ShippingLocationComboInput.First.FillAsync(displayText);

        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    /// <summary>
    /// Chọn giá trị trong combobox DropShip Location.
    /// </summary>
    public async Task SelectDropShipLocationAsync(string displayText)
    {
        await DropShipLocationDropdownButton.First.ClickAsync();

        await DropShipLocationComboInput.First.ClickAsync();
        await DropShipLocationComboInput.First.FillAsync(displayText);

        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });

        await option.First.ClickAsync();
    }

    // ===== Tabs (Locations / Address / Companies) - hỗ trợ cả VI & EN =====

    private ILocator TabItems => _page.Locator("dxbl-tab-item[role='tab']");

    private ILocator GetTabByText(string text) =>
        TabItems.Filter(new LocatorFilterOptions { HasTextString = text });

    public ILocator TabViTri => GetTabByText("Vị Trí").Or(GetTabByText("Locations"));
    public ILocator TabDiaChi =>
        GetTabByText("Địa Chỉ").Or(GetTabByText("Address")).Or(GetTabByText("Addresses"));
    public ILocator TabNhaPhanPhoi =>
        GetTabByText("Nhà Phân Phối").Or(GetTabByText("Companies")).Or(GetTabByText("Distributors"));

    private ILocator GetTabByTextWithFallback(string text) =>
        text switch
        {
            "Vị Trí" => GetTabByText("Vị Trí").Or(GetTabByText("Locations")),
            "Địa Chỉ" => GetTabByText("Địa Chỉ").Or(GetTabByText("Address")).Or(GetTabByText("Addresses")),
            "Nhà Phân Phối" =>
                GetTabByText("Nhà Phân Phối").Or(GetTabByText("Companies")).Or(GetTabByText("Distributors")),
            _ => GetTabByText(text)
        };

    public async Task SelectTabAsync(string text)
    {
        var tab = GetTabByTextWithFallback(text).First;
        await tab.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await tab.ClickAsync();
        await Assertions.Expect(tab).ToHaveAttributeAsync("aria-selected", "true");
    }

    // ===== Companies tab (nhóm + grid trong tab Companies) =====

    /// <summary>Nút Add Companies trong header nhóm <c>Companies</c>.</summary>
    public ILocator ButtonAddCompaniesWarehouse =>
        _page.Locator("button#ne_Btn_AddCompanies_Warehouse[type='button']");

    /// <summary>Checkbox "Exclude selected companies" — <c>name="chk_ExcludeAllCompanies_Warehouse"</c>.</summary>
    public ILocator CheckboxExcludeAllCompaniesWarehouse =>
        _page.Locator("input[type='checkbox'][name='chk_ExcludeAllCompanies_Warehouse']");

    /// <summary>
    /// Nhóm form layout chứa nút <c>ne_Btn_AddCompanies_Warehouse</c> (tránh nhầm grid tab khác).
    /// </summary>
    private ILocator CompaniesFormLayoutGroup =>
        _page.Locator("dxbl-form-layout-group").Filter(new LocatorFilterOptions
        {
            Has = ButtonAddCompaniesWarehouse
        });

    /// <summary>Grid Company trong tab Companies (<c>hqsoft-grid</c> trong cùng nhóm).</summary>
    public ILocator CompaniesGrid => CompaniesFormLayoutGroup.Locator("dxbl-grid.hqsoft-grid");

    /// <summary>Hàng thêm Company inline mới (<c>dxbl-grid-edit-new-item-row-inplace</c>).</summary>
    public ILocator CompaniesGridNewItemRow =>
        CompaniesGrid.Locator("tr.dxbl-grid-edit-new-item-row-inplace");

    /// <summary>Icon cảnh báo required trong ô Company (snippet: <c>validation-warning.svg</c> + title).</summary>
    public ILocator CompaniesGridCompanyRequiredWarning =>
        CompaniesGridNewItemRow.Locator("img[title='This field is required.']");

    public async Task ClickAddCompaniesAsync()
    {
        await ButtonAddCompaniesWarehouse.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await ButtonAddCompaniesWarehouse.First.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    // ===== Modal "Add Companies" (popup chọn công ty) =====
    // Các id/name trong modal thường là GUID; ưu tiên title, data-qa, placeholder, nhãn cột.

    /// <summary>Viền nội dung modal có tiêu đề <c>Add Companies</c>.</summary>
    public ILocator AddCompaniesModal =>
        _page.Locator(".dxbl-modal-content").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("span.dxbl-modal-title").Filter(new LocatorFilterOptions
            {
                HasTextString = "Add Companies"
            })
        });

    public ILocator AddCompaniesModalCloseButton =>
        AddCompaniesModal.Locator("[data-qa-selector='dx-popup-close-button']");

    public ILocator AddCompaniesModalFilterCodeInput =>
        AddCompaniesModal.Locator("dxbl-input-editor[placeholder='Code'] input.dxbl-text-edit-input");

    public ILocator AddCompaniesModalFilterCompanyNameInput =>
        AddCompaniesModal.Locator("dxbl-input-editor[placeholder='Company Name'] input.dxbl-text-edit-input");

    private ILocator AddCompaniesModalCountryFormGroup =>
        AddCompaniesModal.Locator("div.form-group").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("label.form-label").Filter(new LocatorFilterOptions { HasTextString = "Country" })
        });

    private ILocator AddCompaniesModalCountryComboHost =>
        AddCompaniesModalCountryFormGroup.Locator("dxbl-combo-box");

    public ILocator AddCompaniesModalCountryComboInput =>
        AddCompaniesModalCountryComboHost.Locator("input[role='combobox']");

    public ILocator AddCompaniesModalCountryDropdownButton =>
        AddCompaniesModalCountryComboHost.Locator(".dxbl-btn-group-right button");

    private ILocator AddCompaniesModalSelectGeographyFormGroup =>
        AddCompaniesModal.Locator("div.form-group").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("label.form-label").Filter(new LocatorFilterOptions
            {
                HasTextString = "Select Geography"
            })
        });

    private ILocator AddCompaniesModalTerritoryFormGroup =>
        AddCompaniesModal.Locator("div.form-group").Filter(new LocatorFilterOptions
        {
            Has = _page.Locator("label.form-label").Filter(new LocatorFilterOptions { HasTextString = "Territory" })
        });

    /// <summary>Memo đường dẫn Geography (readonly) trong modal.</summary>
    public ILocator AddCompaniesModalGeographyMemo =>
        AddCompaniesModalSelectGeographyFormGroup.Locator("textarea");

    /// <summary>Memo Territory (readonly) trong modal.</summary>
    public ILocator AddCompaniesModalTerritoryMemo =>
        AddCompaniesModalTerritoryFormGroup.Locator("textarea");

    /// <summary>Nút popup <c>...</c> cạnh Geography (có thể <c>disabled</c>).</summary>
    public ILocator AddCompaniesModalGeographyPickerButton =>
        AddCompaniesModalSelectGeographyFormGroup.Locator("button.dxbl-btn-standalone");

    /// <summary>Nút popup <c>...</c> cạnh Territory (có thể <c>disabled</c>).</summary>
    public ILocator AddCompaniesModalTerritoryPickerButton =>
        AddCompaniesModalTerritoryFormGroup.Locator("button.dxbl-btn-standalone");

    public ILocator AddCompaniesModalButtonClearFilters =>
        AddCompaniesModal.GetByRole(AriaRole.Button, new() { Name = "Clear Filters" });

    /// <summary>Search cùng hàng với Clear Filters (bộ lọc phía trên).</summary>
    public ILocator AddCompaniesModalButtonApplyFilterSearch =>
        AddCompaniesModal.Locator("div.row.mt-3.mb-2").GetByRole(AriaRole.Button, new() { Name = "Search" });

    public ILocator AddCompaniesModalQuickSearchInput =>
        AddCompaniesModal.Locator("dxbl-input-editor[placeholder='Search'] input.dxbl-text-edit-input");

    /// <summary>Nút Search cạnh ô quick search (cột 2).</summary>
    public ILocator AddCompaniesModalQuickSearchButton =>
        AddCompaniesModal.Locator("div.row").Filter(new LocatorFilterOptions
        {
            Has = AddCompaniesModal.Locator("dxbl-input-editor[placeholder='Search']")
        }).GetByRole(AriaRole.Button, new() { Name = "Search" });

    public ILocator AddCompaniesModalButtonAdvancedFilters =>
        AddCompaniesModal.GetByRole(AriaRole.Button, new() { Name = "Advanced filters" });

    /// <summary>Bảng kết quả (Bootstrap <c>b-datagrid compact-grid</c>).</summary>
    public ILocator AddCompaniesModalResultsTable =>
        AddCompaniesModal.Locator("table.b-datagrid.compact-grid");

    public ILocator AddCompaniesModalSelectableRows =>
        AddCompaniesModalResultsTable.Locator("tbody tr.table-row-selectable");

    public ILocator AddCompaniesModalButtonCancel =>
        AddCompaniesModal.Locator(".dxbl-modal-footer").GetByRole(AriaRole.Button, new() { Name = "Cancel" });

    public ILocator AddCompaniesModalButtonConfirm =>
        AddCompaniesModal.Locator(".dxbl-modal-footer").GetByRole(AriaRole.Button, new() { Name = "Add Companies" });

    public async Task EnsureAddCompaniesModalVisibleAsync()
    {
        await AddCompaniesModal.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
    }

    /// <summary>Chọn Country trong modal (mở combo, gõ, chọn option).</summary>
    public async Task SelectAddCompaniesModalCountryAsync(string displayText)
    {
        await AddCompaniesModalCountryDropdownButton.First.ClickAsync();
        await AddCompaniesModalCountryComboInput.First.ClickAsync();
        await AddCompaniesModalCountryComboInput.First.FillAsync(displayText);
        await _page.WaitForTimeoutAsync(500);

        ILocator option = _page.GetByRole(AriaRole.Option, new() { Name = displayText });
        if (await option.CountAsync() == 0)
            option = _page.GetByRole(AriaRole.Option).Filter(new LocatorFilterOptions { HasTextString = displayText });
        if (await option.CountAsync() == 0)
            option = _page.Locator(".dxbl-list-box-item", new() { HasText = displayText });

        await option.First.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = _settings.StandardTimeoutMs
        });
        await option.First.ClickAsync();
    }

    // ===== Support methods =====

    public async Task FillCodeAndDescriptionAsync(string code, string description)
    {
        await InputCode.FillAsync(code);
        await InputDescription.FillAsync(description);
    }

    public async Task FillCodeOnlyAsync(string code)
    {
        await InputCode.FillAsync(code);
    }

    public async Task FillDescriptionOnlyAsync(string description)
    {
        await InputDescription.FillAsync(description);
    }

    public async Task SetActiveAsync(bool active)
    {
        var box = CheckboxActive.First;
        if (active)
            await box.CheckAsync();
        else
            await box.UncheckAsync();
    }

    public async Task SetIsPublicAsync(bool isPublic)
    {
        var box = CheckboxIsPublic.First;
        if (isPublic)
            await box.CheckAsync();
        else
            await box.UncheckAsync();
    }
}
