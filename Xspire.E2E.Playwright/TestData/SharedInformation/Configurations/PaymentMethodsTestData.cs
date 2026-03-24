namespace Xspire.E2E.Playwright.TestData.SharedInformation.Configurations;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Configurations &gt; Payment Methods.
/// </summary>
public static class PaymentMethodsTestData
{
    public static class CreateValid
    {
        public const string Code = "AUTOPAYM01";
        public const string MeansOfPayment = "Auto Means E2E";
        public const string Description = "Auto Payment Method E2E";
    }

    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Auto Payment Method E2E edited";
    }
}
