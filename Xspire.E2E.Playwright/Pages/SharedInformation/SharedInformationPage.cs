using System.Threading.Tasks;
using Microsoft.Playwright;
using Xspire.E2E.Playwright.Config;

namespace Xspire.E2E.Playwright.Pages.SharedInformation;

/// <summary>
/// Represents the Shared Information workspace page (Workspace/SI) that shows shortcut links.
/// </summary>
public class SharedInformationPage
{
    private readonly IPage _page;
    private readonly PlaywrightSettings _settings;

    public SharedInformationPage(IPage page, PlaywrightSettings settings)
    {
        _page = page;
        _settings = settings;
    }

    // Header "Shared Information" tab - breadcrumb; tạm thời bắt theo text.
    public ILocator SharedInformationTab =>
        _page.GetByText("Shared Information", new() { Exact = false });

    #region Geo Subdivisions
    // Link shortcut "Countries" trong khu Your Shortcuts (không bắt heading h1).
    public ILocator CountriesLink =>
        _page.GetByText("Countries", new() { Exact = true });

    public ILocator GeographiesLink =>
        _page.GetByText("Geographies", new() { Exact = true });

    public ILocator GeographyLevelDefinitionsLink =>
        _page.GetByText("Geography Level Definitions", new() { Exact = true });
    #endregion

    #region Customer Classification
    public ILocator LocationsLink =>
        _page.GetByText("Locations", new() { Exact = true });

    public ILocator SaleChannelsLink =>
        _page.GetByText("Sale Channels", new() { Exact = true });
    #endregion

    #region Shipping
    public ILocator FobsLink =>
        _page.GetByText("Fobs", new() { Exact = true });

    public ILocator ShippingTermsLink =>
        _page.GetByText("Shipping Terms", new() { Exact = true });

    public ILocator ShippingZonesLink =>
        _page.GetByText("Shipping Zones", new() { Exact = true });

    public ILocator ShipViasLink =>
        _page.GetByText("Ship Vias", new() { Exact = true });
    #endregion

    #region Configurations
    public ILocator TimeZoneLink =>
        _page.GetByText("Time Zone", new() { Exact = true });

    public ILocator AttributesLink =>
        _page.GetByText("Attributes", new() { Exact = true });

    public ILocator CreditTermsLink =>
        _page.GetByText("Credit Terms", new() { Exact = true });

    public ILocator PaymentMethodsLink =>
        _page.GetByText("Payment Methods", new() { Exact = true });

    public ILocator WorkCalendarsLink =>
        _page.GetByText("Work Calendars", new() { Exact = true });

    public ILocator ReasonCodesLink =>
        _page.GetByText("Reason Codes", new() { Exact = true });

    public ILocator NumberingsLink =>
        _page.GetByText("Numberings", new() { Exact = true });

    public ILocator CodeGeneratingsLink =>
        _page.GetByText("Code Generatings", new() { Exact = true });

    public ILocator HolidaysLink =>
        _page.GetByText("Holidays", new() { Exact = true });
    #endregion

    #region Geo Sales Structure
    public ILocator TerritoriesLink =>
        _page.GetByText("Territories", new() { Exact = true });

    public ILocator TerritoryLevelDefinitionsLink =>
        _page.GetByText("Territory Level Definitions", new() { Exact = true });
    #endregion

    #region Workflow
    public ILocator WorkflowActionsLink =>
        _page.GetByText("Workflow Actions", new() { Exact = true });

    public ILocator WorkflowStatesLink =>
        _page.GetByText("Workflow States", new() { Exact = true });

    public ILocator WorkflowsLink =>
        _page.GetByText("Workflows", new() { Exact = true });
    #endregion

    #region Taxes
    public ILocator TaxCategoriesLink =>
        _page.GetByText("Tax Categories", new() { Exact = true });

    public ILocator TaxesLink =>
        _page.GetByText("Taxes", new() { Exact = true });
    #endregion

    #region Navigation helpers
    public async Task NavigateToCountriesAsync()
    {
        await CountriesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToGeographiesAsync()
    {
        await GeographiesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToGeographyLevelDefinitionsAsync()
    {
        await GeographyLevelDefinitionsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToLocationsAsync()
    {
        await LocationsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToSaleChannelsAsync()
    {
        await SaleChannelsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToFobsAsync()
    {
        await FobsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToShippingTermsAsync()
    {
        await ShippingTermsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToShippingZonesAsync()
    {
        await ShippingZonesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToShipViasAsync()
    {
        await ShipViasLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToTimeZoneAsync()
    {
        await TimeZoneLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToAttributesAsync()
    {
        await AttributesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToCreditTermsAsync()
    {
        await CreditTermsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToPaymentMethodsAsync()
    {
        await PaymentMethodsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToWorkCalendarsAsync()
    {
        await WorkCalendarsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToReasonCodesAsync()
    {
        await ReasonCodesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToNumberingsAsync()
    {
        await NumberingsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToCodeGeneratingsAsync()
    {
        await CodeGeneratingsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToHolidaysAsync()
    {
        await HolidaysLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToTerritoriesAsync()
    {
        await TerritoriesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToTerritoryLevelDefinitionsAsync()
    {
        await TerritoryLevelDefinitionsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToWorkflowActionsAsync()
    {
        await WorkflowActionsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToWorkflowStatesAsync()
    {
        await WorkflowStatesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToWorkflowsAsync()
    {
        await WorkflowsLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToTaxCategoriesAsync()
    {
        await TaxCategoriesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }

    public async Task NavigateToTaxesAsync()
    {
        await TaxesLink.ClickAsync();
        await _page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
    }
    #endregion
}

