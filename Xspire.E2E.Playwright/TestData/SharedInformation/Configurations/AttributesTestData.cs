namespace Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Configurations &gt; Attributes.
/// </summary>
public static class AttributesTestData
{
    public static class CreateValid
    {
        public const string Code = "AUTOATTR10";
        public const string Description = "Auto Attribute Checkbox E2E";
        public const string ControlTypeSearchText = "Checkbox";
        public const string ControlTypeOptionText = "Checkbox";

        public const string EntryMask = "";
        public const string RegExp = "";

        public const string ValueRow1Code = "GT01";
        public const string ValueRow1Description = "Giá trị 1";
        public const string ValueRow1SortOrder = "0";

        public const string ValueRow2Code = "GT02";
        public const string ValueRow2Description = "Giá trị 2";
        public const string ValueRow2SortOrder = "1";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Auto Attribute Checkbox E2E edited";
    }
}
