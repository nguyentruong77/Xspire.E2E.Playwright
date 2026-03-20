namespace Xspire.E2E.Playwright.TestData.SharedInformation.CustomerClassification;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Customer Classification &gt; Locations.
/// </summary>
public static class LocationsTestData
{
    public static class CreateValid
    {
        // Auto-test meaningful uppercase code (~10 chars).
        public const string Code = "LOCAUTO10A";

        public const string Description = "Auto test Locations LOCAUTO10A";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription =
            $"{CreateValid.Description} - TC_LOCATIONS_EDIT";
    }
}

