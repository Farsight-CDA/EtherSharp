using System;

namespace EtherSharp.Generator.SyntaxElements;
public enum FieldVisibility
{
    Public,
    Private,
    Internal
}
public class FieldBuilder : ISyntaxBuilder
{
    public string Type { get; }
    public string Name { get; }
    public string? DefaultValue { get; private set; }
    public FieldVisibility Visibility { get; private set; } = FieldVisibility.Public;

    public bool IsReadonly { get; private set; }
    public bool IsStatic { get; private set; }

    public FieldBuilder(string type, string name)
    {
        Type = type;
        Name = name;
    }

    public FieldBuilder WithVisibility(FieldVisibility visibility)
    {
        Visibility = visibility;
        return this;
    }

    public FieldBuilder WithIsReadonly(bool isReadonly = true)
    {
        IsReadonly = isReadonly;
        return this;
    }

    public FieldBuilder WithIsStatic(bool isStatic = true)
    {
        IsStatic = isStatic;
        return this;
    }

    public FieldBuilder WithDefaultValue(string defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public string Build()
        => $$"""
            {{Visibility.ToString().ToLower()}}{{(IsStatic ? " static" : "")}}{{(IsReadonly ? " readonly" : "")}} {{Type}} {{Name}} {{(DefaultValue is null ? "" : $"= {DefaultValue}")}};
            """;

    public SyntaxId GetSyntaxId()
    {
        int hashCode = HashCode.Combine(
            nameof(FieldBuilder),
            Type,
            Name,
            Visibility,
            IsReadonly
        );
        return new SyntaxId(hashCode);
    }
}
