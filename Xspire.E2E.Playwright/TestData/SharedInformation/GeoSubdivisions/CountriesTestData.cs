namespace Xspire.E2E.Playwright.TestData.SharedInformation.GeoSubdivisions;

/// <summary>
/// Hard-coded test data cho Shared Information &gt; Geo Subdivisions &gt; Countries.
/// Tương tự cấu trúc TaxCategoriesTestData: gom các bộ dữ liệu theo từng ngữ cảnh (CreateValid, ...)
/// </summary>
public static class CountriesTestData
{
    public static class CreateValid
    {
        // Bạn có thể chỉnh lại cho đúng rule business (ví dụ code format, description, v.v.)
        public const string Code = "CPC";

        // Text hiển thị của Time Zone trong combobox – phải trùng tên option trong list
        // (VD: "Coordinated Universal Time" cho UTC, "SE Asia Standard Time" cho UTC+7).
        public const string TimeZoneText = "Coordinated Universal Time";

        public const string Description = "Auto test country Cambodia";
    }

    /// <summary>Data dùng cho test search: mã cần tìm (thường trùng CreateValid.Code sau khi đã tạo).</summary>
    public static class SearchSuccess
    {
        public const string Code = "CPC";
    }

     /// <summary>Data dùng cho test edit: Description mới sau khi chỉnh sửa.</summary>
     public static class EditDescription
     {
         // Mô tả sau khi edit; có thể là mô tả cũ + suffix.
         public const string NewDescription = $"{CreateValid.Description} - TC_COUNTRY_EDIT_001";
     }
}

