namespace Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

/// <summary>
/// Hard-coded test data for Shared Information > Taxes > Taxes.
/// </summary>
public static class TaxesTestData
{
    public static class CreateValid
    {
        public const string Code = "AUTOTAX10";
        public const string Description = "Auto Tax 10 percent";
        public const string TaxTypeSearchText = "VAT";
        public const string CalculationRuleSearchText = "Document";
        public const string TaxScheduleDate = "04/30/2030";
        public const string TaxScheduleRate = "10";
        public const string TaxCategorySearchText = "TESTTAX";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Auto Tax 10 percent edited";
    }
}
