namespace Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

/// <summary>
/// Hard-coded test data for Shared Information > Configurations > Reason Codes.
/// </summary>
public static class ReasonCodesTestData
{
    public static class CreateValid
    {
        public const string Code = "AUTORSN01";
        public const string Description = "Auto Reason Code E2E";
        public const string TypeSearchText = "Issue";
        public const string TypeOptionText = "Issue";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Auto Reason Code E2E edited";
    }
}
