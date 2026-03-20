namespace Xspire.E2E.Playwright.TestData.SharedInformation.GeoSubdivisions;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Geo Subdivisions &gt; Geography Level Definitions.
/// </summary>
public static class GeographyLevelDefinitionsTestData
{
    public static class CreateValid
    {
        // Auto-test meaningful uppercase code (~10 chars).
        public const string Code = "GEOAUTO10A";

        public const string Description = "Auto test Geography Level Definitions GEOAUTO10A";

        // Must be consistent with Level Details (row Level=1).
        public const string MaxLevel = "1";

        public const string LevelDescription = "Level 1 - GEOAUTO10A";

        // Combobox Country có input; filter theo text này để chọn item đầu tiên.
        public const string CountrySearchText = "TESTFORGLD";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = $"{CreateValid.Description} - TC_GEOGRAPHY_EDIT";
    }
}

