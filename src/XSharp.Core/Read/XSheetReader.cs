﻿using System.Reflection;
using XSharp.Shared;

namespace XSharp.Core.Read;

public static class XSheetReader
{
    private static readonly IEasLog logger = EasLogFactory.CreateLogger();

    internal static ResultData<XSheet<object>> Read(ExcelWorksheet? sheet, Type type)
    {
        if (sheet is null) return Result.Warn("Worksheet is null");
        if (sheet.Dimension is null) return Result.Warn("Worksheet dimension is null or empty");
        var headers = sheet.GetHeaders();
        if (headers.Count == 0) return Result.Warn("Worksheet has no valid headers");
        if (sheet.Name.IsNullOrEmpty()) return Result.Warn("WorkSheet name is null or empty");
        var fixedName = sheet.Name.FixName();
        if (fixedName.IsNullOrEmpty()) return Result.Warn("FixedSheetName is null or empty. SheetName: " + sheet.Name);
        var xSheet = new XSheet<object>();
        xSheet.Name = sheet.Name;
        xSheet.Dimension = sheet.Dimension;
        xSheet.FixedName= fixedName;
        xSheet.Headers= headers;
        var rows = ReadRows(headers, sheet, type);
        xSheet.Rows = rows;
        return xSheet;
    }

    public static ResultData<XSheet<T>> Read<T>(ExcelWorksheet? sheet)
    {
        if (sheet is null) return Result.Warn("Worksheet is null");
        if (sheet.Dimension is null) return Result.Warn("Worksheet dimension is null or empty");
        var headers = sheet.GetHeaders();
        if (headers.Count == 0) return Result.Warn("Worksheet has no valid headers");
        if (sheet.Name.IsNullOrEmpty()) return Result.Warn("WorkSheet name is null or empty");
        var fixedName = sheet.Name.FixName();
        if (fixedName.IsNullOrEmpty()) return Result.Warn("FixedSheetName is null or empty. SheetName: " + sheet.Name);
        var xSheet = new XSheet<T>();
        xSheet.Name = sheet.Name;
        xSheet.Dimension = sheet.Dimension;
        xSheet.FixedName= fixedName;
        xSheet.Headers= headers;
        var rows = ReadRows<T>(headers, sheet);
        xSheet.Rows = rows;
        return xSheet;
    }

    private static List<object> ReadRows(IReadOnlyCollection<IXHeader> headers, ExcelWorksheet? sheet, Type type)
    {
        var start = sheet.Dimension.Start;
        var end = sheet.Dimension.End;
        var rows = new List<object>();
        var validator = XOptionLib.This.GetValidator();
        for (var row = start.Row; row <= end.Row; row++)
        {
            if (row == XOptionLib.This.Option.HeaderColumnNumber) continue;
            var item = Activator.CreateInstance(type)!;
            var isSetAnyValue = false;
            for (var col = start.Column; col <= end.Column; col++)
                try
                {
                    var cell = sheet.Cells[row, col];
                    var value = cell.Value;
                    if (validator?.IsIgnoreCell(value) == true) continue;
                    value = validator?.GetValidCellValue(value) ?? value;
                    var currentHeader = headers.FirstOrDefault(x => x.Index == col - 1);
                    if (currentHeader is null) continue;
                    var property = type.GetProperty(currentHeader.Name,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property is null) continue;
                    var isConvertSuccess =
                        XValueConverter.TryConvert(value.ToString(), property.PropertyType, out var convertedVal);
                    if (!isConvertSuccess || convertedVal is null) continue;
                    property.SetValue(item, value);
                    isSetAnyValue = true;
                }
                catch (Exception ex)
                {
                    logger.Exception(ex, "Error while reading table");
                }

            if (!isSetAnyValue) continue;
            rows.Add(item);
        }

        return rows;
    }

    private static List<T> ReadRows<T>(IReadOnlyCollection<IXHeader> headers, ExcelWorksheet sheet)
    {
        var start = sheet.Dimension.Start;
        var end = sheet.Dimension.End;
        var rows = new List<T>();
        var validator = XOptionLib.This.GetValidator();
        var type = typeof(T);
        for (var row = start.Row; row <= end.Row; row++)
        {
            if (row == XOptionLib.This.Option.HeaderColumnNumber) continue;
            var item = (T)Activator.CreateInstance(type)!;
            var isSetAnyValue = false;
            for (var col = start.Column; col <= end.Column; col++)
                try
                {
                    var cell = sheet.Cells[row, col];
                    var value = cell.Value;
                    if (value is null) continue;
                    if (validator.IsIgnoreCell(value) == true) continue;
                    value = validator?.GetValidCellValue(value) ?? value;
                    var currentHeader = headers.FirstOrDefault(x => x.Index == col - 1);
                    if (currentHeader is null) continue;
                    var property = type.GetProperty(currentHeader.FixedName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (property is null) continue;
                    var isConvertSuccess =
                        XValueConverter.TryConvert(value.ToString(), property.PropertyType, out var convertedVal);
                    if (!isConvertSuccess || convertedVal is null) continue;
                    property.SetValue(item, convertedVal);
                    isSetAnyValue = true;
                }
                catch (Exception ex)
                {
                    logger.Exception(ex, "Error while reading table");
                }

            if (!isSetAnyValue) continue;
            rows.Add(item);
        }

        return rows;
    }
}