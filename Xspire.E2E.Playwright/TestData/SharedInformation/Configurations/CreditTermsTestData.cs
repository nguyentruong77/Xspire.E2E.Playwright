namespace Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Configurations &gt; Credit Terms.
/// </summary>
public static class CreditTermsTestData
{
    public static class CreateValid
    {
        public const string Code = "AUTOCRTERM01";
        public const string Description = "Auto Credit Term E2E";
        public const string ApplyToSearchText = "All";
        public const string DueDateTypeOptionText = "Fixed Number of Days";
        public const string DueDay = "7";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Auto Credit Term E2E edited";
    }
}
