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
