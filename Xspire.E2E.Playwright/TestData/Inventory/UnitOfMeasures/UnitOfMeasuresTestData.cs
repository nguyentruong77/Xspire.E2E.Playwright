namespace Xspire.E2E.Playwright.TestData.Inventory.UnitOfMeasures;

/// <summary>
/// Hard-coded test data cho Inventory &gt; Unit Of Measures.
/// Cấu trúc tương tự TaxCategoriesTestData và CountriesTestData (SharedInformation).
/// </summary>
public static class UnitOfMeasuresTestData
{
    /// <summary>Dữ liệu cho các test thiếu/rỗng (TC-UOM-002–004).</summary>
    public static class RequiredValidation
    {
        public const string FromUnit = "REQ_UOM_001";
        public const string ToUnit = "REQ_BOX_001";
    }

    /// <summary>Kịch bản tạo mới thành công (TC-UOM-005).</summary>
    public static class CreateSuccess
    {
        public const string FromUnit = "AUTO_UOM_CREATE_001";
        public const string ToUnit = "AUTO_BOX_CREATE_001";
    }

    /// <summary>Kịch bản duplicate / tìm kiếm (TC-UOM-006–008).</summary>
    public static class DuplicateScenario
    {
        public const string FromUnit = "A";
        public const string ToUnit = "AUTO_BOX_DUP_001";
    }
}
