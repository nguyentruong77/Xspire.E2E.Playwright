namespace Xspire.E2E.Playwright.TestData.SharedInformation.GeoSalesStructure;

/// <summary>
/// Shared Information &gt; Geo Sales Structure &gt; Territory Level Definitions.
/// </summary>
public static class TerritoryLevelDefinitionsTestData
{
    public static class CreateValid
    {
        public const string Code = "TLDAUTO10A";

        public const string Description = "Auto test Territory Level Definitions TLDAUTO10A";

        public const string MaxLevel = "1";

        public const string LevelDescription = "Level 1 - TLDAUTO10A";

        /// <summary>Filter Country combobox; then select first list item.</summary>
        public const string CountrySearchText = "VN";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = $"{CreateValid.Description} - TC_TLD_EDIT";
    }
}
