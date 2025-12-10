using Microsoft.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace EtherSharp.Generator.Util;

public static class NameUtils
{
    public static string ToValidParameterName(string name)
        => EscapeVariableName(Uncapitalize(name).Trim('_'));

    public static string ToValidVariableName(string name)
        => EscapeVariableName(Uncapitalize(name));

    public static string ToValidPropertyName(string name)
        => EscapeVariableName(ToValidNamespaceName(Capitalize(name)));

    public static string ToValidNamespaceName(string name)
    {
        var finalSb = new StringBuilder();
        string[] parts = ReplaceAll(name, '_', '.', ' ', '/', '\\', '+')
            .Split(['_'], StringSplitOptions.RemoveEmptyEntries);

        foreach(string? part in parts)
        {
            _ = finalSb.Append(Capitalize(part));
        }

        return finalSb.ToString();
    }

    public static string ToValidClassName(string name)
        => ToValidNamespaceName(name);

    public static string ToValidFunctionName(string name)
        => ToValidNamespaceName(name);

    public static string Uncapitalize(string name)
        => name.Length == 0
            ? ""
            : name[0].ToString().ToLower(CultureInfo.InvariantCulture) + name.Substring(1);

    public static string Capitalize(string name)
        => name.Length == 0
            ? ""
            : name[0].ToString().ToUpper(CultureInfo.InvariantCulture) + name.Substring(1);

    public static string ReplaceAll(string name, char replacement, params char[] oldChars)
    {
        foreach(char oldChar in oldChars)
        {
            name = name.Replace(oldChar, replacement);
        }

        return name;
    }

    private static readonly HashSet<string> _reservedKeywords = new(
        StringComparer.Ordinal)
    {
        "abstract","as","base","bool","break","byte","case","catch","char",
        "checked","class","const","continue","decimal","default","delegate","do",
        "double","else","enum","event","explicit","extern","false","finally","fixed",
        "float","for","foreach","goto","if","implicit","in","int","interface","internal",
        "is","lock","long","namespace","new","null","object","operator","out","override",
        "params","private","protected","public","readonly","ref","return","sbyte",
        "sealed","short","sizeof","stackalloc","static","string","struct","switch","this",
        "throw","true","try","typeof","uint","ulong","unchecked","unsafe","ushort",
        "using","virtual","void","volatile","while"
    };
    public static string EscapeVariableName(string name)
        => name is null
            ? throw new ArgumentNullException(nameof(name))
            : _reservedKeywords.Contains(name)
                ? "@" + name
                : name;

    public static string FullyQualifiedTypeName(ITypeSymbol symbol)
    {
        var sb = new StringBuilder();

        _ = sb.Append($"global::{symbol.ContainingNamespace}");

        var parentNames = new List<string>();
        var parentType = symbol.ContainingType;
        while(parentType is not null)
        {
            parentNames.Add(parentType.Name);
            parentType = parentType.ContainingType;
        }

        parentNames.Reverse();
        foreach(string parentName in parentNames)
        {
            _ = sb.Append($".{parentName}");
        }

        _ = sb.Append($".{symbol.Name}");

        if(symbol is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
        {
            _ = sb.Append('<');

            foreach(var typeArg in namedType.TypeArguments)
            {
                _ = sb.Append(FullyQualifiedTypeName(typeArg));
            }

            _ = sb.Append('>');
        }

        return sb.ToString();
    }
}
