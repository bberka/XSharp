﻿namespace XSharp.Core.Manager;

internal static class XSheetModelBuilder
{
  public static void ExportSharpModel(List<XHeader> headers, string realSheetName, string fixedName, string outPath) {
    XPathLib.CreateDirectory(outPath);
    var fileOutPath = Path.Combine(outPath, $"{fixedName}.cs");
    if (File.Exists(fileOutPath)) throw new Exception("File already exists: " + fileOutPath);
    var fileContent = CreateSharpModel(headers, realSheetName, fixedName);
    WriteToFile(fileOutPath, fileContent);
  }

  internal static void WriteToFile(string outPath, string content) {
    XPathLib.CreateDirectory(outPath);
    File.WriteAllText(outPath, content);
  }

  private static string CreateSharpModel(List<XHeader> headers, string realSheetName, string fixedSheetName) {
    var sb = new StringBuilder();
    XBuilderHelper.AppendUsingList(sb);
    XBuilderHelper.AppendNamespace(sb, true);
    XBuilderHelper.AppendXSheetNameAttribute(sb, realSheetName, false);
    XBuilderHelper.AppendClassStart(sb, fixedSheetName, true);
    foreach (var col in headers) {
      var valueType = col.ValueType?.Name;

      XBuilderHelper.AppendPropertySummaryIfExists(sb, col.Comment);
      XBuilderHelper.AppendXHeaderNameAttribute(sb, col.Name, col.FixedName);
      XBuilderHelper.AppendHeaderIndexAttribute(sb, col.Index);
      if (valueType is null) {
        valueType = XOptionLib.This.Option.DefaultValueType.ToString();
        XBuilderHelper.AppendXCellValueTypeInvalidAttribute(sb, valueType);
      }

      XBuilderHelper.AppendProperty(sb, valueType, fixedSheetName, col.FixedName);
    }

    XBuilderHelper.AppendEnd(sb);
    return sb.ToString();
  }
}