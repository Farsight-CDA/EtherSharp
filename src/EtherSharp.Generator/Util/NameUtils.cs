using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Globalization;
using System.Text;

namespace EtherSharp.Generator.Util;

internal static class NameUtils
{
    public static string ToValidParameterName(string name)
        => EscapeIdentifier(Uncapitalize(name).Trim('_'));

    public static string ToValidVariableName(string name)
        => EscapeIdentifier(Uncapitalize(name));

    public static string ToValidPropertyName(string name)
        => EscapeIdentifier(ToValidNamespaceName(Capitalize(name)));

    public static string ToValidNamespaceName(string name)
    {
        var finalSb = new StringBuilder();
        string[] parts = ReplaceAll(name, '_', '.', ' ', '/', '\\', '+')
            .Split(['_'], StringSplitOptions.RemoveEmptyEntries);

        foreach(string? part in parts)
        {
            finalSb.Append(Capitalize(part));
        }

        return finalSb.ToString();
    }

    public static string ToValidClassName(string name)
        => ToValidNamespaceName(name);

    public static string ToValidFunctionName(string name)
        => ToValidNamespaceName(name);

    public static string ToValidFileName(string name)
        => ReplaceAll(name, '_', '<', '>', ':', '"', '/', '\\', '|', '?', '*', '\0');

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

    public static string MakeUniquePropertyName(string name, HashSet<string> usedNames)
    {
        if(usedNames.Add(name))
        {
            return name;
        }

        name = $"_{name}";
        while(!usedNames.Add(name))
        {
            name = $"_{name}";
        }

        return name;
    }

    public static string EscapeIdentifier(string identifier)
    {
        identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));

        return SyntaxFacts.GetKeywordKind(identifier) != SyntaxKind.None
            || SyntaxFacts.GetContextualKeywordKind(identifier) != SyntaxKind.None
                ? $"@{identifier}"
                : identifier;
    }

    public static string FullyQualifiedTypeName(ITypeSymbol symbol)
    {
        var sb = new StringBuilder();

        sb.Append($"global::{symbol.ContainingNamespace}");

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
            sb.Append($".{parentName}");
        }

        sb.Append($".{symbol.Name}");

        if(symbol is INamedTypeSymbol namedType && namedType.TypeArguments.Length > 0)
        {
            sb.Append('<');

            foreach(var typeArg in namedType.TypeArguments)
            {
                sb.Append(FullyQualifiedTypeName(typeArg));
            }

            sb.Append('>');
        }

        return sb.ToString();
    }
}
