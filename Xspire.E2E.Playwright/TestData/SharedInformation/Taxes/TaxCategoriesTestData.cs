namespace Xspire.E2E.Playwright.TestData.SharedInformation.Taxes;

/// <summary>
/// Hard-coded test data for Shared Information &gt; Taxes &gt; Tax Categories.
/// Mirrors the structure that was previously in TaxCategoriesTestData.json.
/// </summary>
public static class TaxCategoriesTestData
{
    public static class CreateValid
    {
        public const string Code = "VAT500";
        public const string Description = "Test Nhóm VAT 500%";
    }

    public static class CreateMissingCode
    {
        public const string Code = "";
        public const string Description = "Missing code should trigger validation";
    }

    public static class CreateMissingDescription
    {
        public const string Code = "VAT500";
        public const string Description = "";
    }

    public static class CreateDuplicateCode
    {
        // Trùng Code với CreateValid, khác Description
        public const string Code = "VAT500";
        public const string Description = "Test Nhóm VAT 500% (duplicate)";
    }

    /// <summary>Code dùng search/edit/delete sau khi tạo bằng <see cref="CreateValid"/>.</summary>
    public static class SearchSuccess
    {
        public const string Code = CreateValid.Code;
    }

    public static class EditDescription
    {
        public const string NewDescription = "Test Nhóm VAT 500% (edited)";
    }
}

