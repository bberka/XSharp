﻿using System.Xml.Serialization;
using ExcelToCSharpModelConverter.Shared.Models.Option;
using Microsoft.Extensions.Logging;
using ValueType = ExcelToCSharpModelConverter.Shared.Constants.ValueType;

namespace ExcelToCSharpModelConverter.Shared.Models;

[Serializable]
[XmlRoot("ExportOption")]
public class ExportOption
{
    public string? InFolderPath { get; set; }
    public string? OutFolderPath { get; set; }

    public string NameSpace { get; set; } = "ExcelToCSharpModelConverter.ExportedModels";
    public LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    public ValueType DefaultValueType { get; set; } = ValueType.String;

    public string ModelInheritanceString { get; set; } = ": BaseSheetModel";


    public SetterType SetterTypeString { get; set; } = SetterType.Set;
    public AccessModifierType SetterAccessModifier { get; set; } = AccessModifierType.None;
    public AccessModifierType GetterAccessModifier { get; set; } = AccessModifierType.None;
    public AccessModifierType ClassAccessModifier { get; set; } = AccessModifierType.Public;

    public AccessModifierType ConstructorAccessModifier { get; set; } = AccessModifierType.Public;

    // public string CreateConstructorInitializer { get; set; }
    public ModelType CreateModelAs { get; set; } = ModelType.Class;

    public long HeaderColumnIndex { get; set; } = 1;
    public int SetValueTypesAtRowIndex { get; set; } = 2;

    public bool IgnoreExceptions { get; set; } = false;
    public bool IgnoreFormulas { get; set; } = false;


    public List<string> UsingList { get; set; } = new List<string>();
    public List<string> NullValueStrings { get; set; } = new List<string>();

    public List<IgnoreWhen> IgnoreWhenConditions { get; set; } = new List<IgnoreWhen>();
    public List<ReplaceWhen> ReplaceWhenConditions { get; set; } = new List<ReplaceWhen>();
    public List<TrimWhen> TrimWhenConditions { get; set; } = new List<TrimWhen>();
}