using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

public static class ExcelExtension
{
    /// <summary>
    /// Đọc file Excel và ánh xạ dữ liệu thành danh sách đối tượng.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu đầu ra</typeparam>
    /// <param name="file">File Excel được tải lên</param>
    /// <param name="columnMapping">Dictionary ánh xạ: Tên cột trong Excel -> Tên thuộc tính trong class</param>
    /// <param name="startRow">Dòng bắt đầu đọc dữ liệu (mặc định là dòng 2)</param>
    /// <returns>Danh sách đối tượng đã ánh xạ</returns>
    public static List<T> ImportFromExcel<T>(
        Stream fileStream,
        Dictionary<string, string> columnMapping,
        int startRow = 2) where T : new()
    {
        var result = new List<T>();

        using var workbook = new XLWorkbook(fileStream);
        var worksheet = workbook.Worksheets.First();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Tạo map cột: index -> property
        var columnIndexToProperty = new Dictionary<int, PropertyInfo>();
        var headerRow = worksheet.Row(1);

        for (int col = 1; col <= headerRow.Cells().Count(); col++)
        {
            var headerText = headerRow.Cell(col).GetString().Trim();
            if (columnMapping.ContainsKey(headerText))
            {
                var propName = columnMapping[headerText];
                var prop = properties.FirstOrDefault(p => string.Equals(p.Name, propName, StringComparison.OrdinalIgnoreCase));
                if (prop != null)
                {
                    columnIndexToProperty[col] = prop;
                }
            }
        }

        // Đọc dữ liệu từ dòng startRow
        for (int row = startRow; row <= worksheet.LastRowUsed().RowNumber(); row++)
        {
            var item = new T();
            var currentRow = worksheet.Row(row);

            foreach (var kvp in columnIndexToProperty)
            {
                int colIndex = kvp.Key;
                var prop = kvp.Value;
                var cell = currentRow.Cell(colIndex);

                try
                {
                    object? converted = ConvertCellValue(cell, prop.PropertyType);
                    prop.SetValue(item, converted);
                }
                catch
                {
                    // Bỏ qua lỗi cell, hoặc log nếu cần
                }
            }

            result.Add(item);
        }

        return result;
    }

    /// <summary>
    /// Chuyển giá trị từ cell Excel sang đúng kiểu dữ liệu thuộc tính.
    /// </summary>
    private static object? ConvertCellValue(IXLCell cell, Type targetType)
    {
        var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (nonNullableType == typeof(string))
            return cell.GetString();

        if (nonNullableType == typeof(int))
            return cell.GetValue<int>();

        if (nonNullableType == typeof(decimal))
            return cell.GetValue<decimal>();

        if (nonNullableType == typeof(double))
            return cell.GetValue<double>();

        if (nonNullableType == typeof(float))
            return cell.GetValue<float>();

        if (nonNullableType == typeof(DateTime))
            return cell.GetDateTime();

        if (nonNullableType == typeof(bool))
            return cell.GetValue<bool>();

        // fallback
        return Convert.ChangeType(cell.Value, nonNullableType);
    }


    /// <summary>
    /// Xuất danh sách bất kỳ ra file Excel và lưu vào thư mục chỉ định.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu của danh sách</typeparam>
    /// <param name="data">Danh sách dữ liệu</param>
    /// <param name="sheetName">Tên sheet</param>
    /// <param name="columnMapping">Dictionary map tên cột hiển thị với property name</param>
    /// <param name="fileName">Tên file (không kèm đường dẫn)</param>
    /// <param name="filePath">Đường dẫn thư mục lưu file (tính từ wwwroot)</param>
    /// <returns>Đường dẫn tương đối tới file Excel</returns>

    public static string ExportToExcel<T>(
    List<T> data,
    Dictionary<string, (string PropertyName, string? Format)> columnMapping,
    string sheetName = "Sheet1",
    string fileName = "FileName.xlsx",
    string filePath = "/Export")
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        var properties = typeof(T).GetProperties();
        var usedProperties = columnMapping.Select(kvp =>
        {
            var prop = properties.FirstOrDefault(p => p.Name == kvp.Value.PropertyName);
            return (DisplayName: kvp.Key, Property: prop, Format: kvp.Value.Format);
        }).Where(x => x.Property != null).ToList();

        int columnOffset = 1;
        worksheet.Cell(1, 1).Value = "STT";
        worksheet.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.LightGreen;
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // Header
        for (int colIndex = 0; colIndex < usedProperties.Count; colIndex++)
        {
            var cell = worksheet.Cell(1, colIndex + columnOffset + 1);
            cell.Value = usedProperties[colIndex].DisplayName;
            cell.Style.Font.Bold = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightGreen;
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        }

        // Data
        for (int rowIndex = 0; rowIndex < data.Count; rowIndex++)
        {
            var item = data[rowIndex];

            worksheet.Cell(rowIndex + 2, 1).Value = rowIndex + 1;
            worksheet.Cell(rowIndex + 2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            for (int colIndex = 0; colIndex < usedProperties.Count; colIndex++)
            {
                var (displayName, prop, format) = usedProperties[colIndex];
                var value = prop.GetValue(item);
                var cell = worksheet.Cell(rowIndex + 2, colIndex + columnOffset + 1);

                if (value is DateTime dt && !string.IsNullOrEmpty(format))
                {
                    cell.Value = dt.ToString(format);
                }
                else
                {
                    cell.Value = value?.ToString() ?? "";
                }

                // Căn chỉnh
                if (value is int || value is float || value is double || value is decimal)
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                else if (value is DateTime)
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                else
                    cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            }
        }

        worksheet.Columns().AdjustToContents();

        var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filePath.TrimStart('/'));
        if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
        var fullPath = Path.Combine(rootPath, fileName);
        workbook.SaveAs(fullPath);
        return Path.Combine(filePath, fileName).Replace("\\", "/");
    }

}
