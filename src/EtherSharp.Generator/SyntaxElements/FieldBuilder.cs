using System.Globalization;

namespace EtherSharp.Generator.SyntaxElements;

public enum FieldVisibility
{
    Public,
    Private,
    Internal
}
public class FieldBuilder(string type, string name) : ISyntaxBuilder
{
    public string Type { get; } = type;
    public string Name { get; } = name;
    public string? DefaultValue { get; private set; }
    public FieldVisibility Visibility { get; private set; } = FieldVisibility.Public;

    public bool IsReadonly { get; private set; }
    public bool IsStatic { get; private set; }

    public string? XmlSummaryContent { get; private set; }

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

    public FieldBuilder WithXmlSummaryContent(string? xmlSummaryContent)
    {
        XmlSummaryContent = xmlSummaryContent;
        return this;
    }

    public string Build()
        => $$"""
            {{BuildXmlComment()}}{{Visibility.ToString().ToLower(CultureInfo.InvariantCulture)}}{{(IsStatic ? " static" : "")}}{{(IsReadonly ? " readonly" : "")}} {{Type}} {{Name}} {{(DefaultValue is null ? "" : $"= {DefaultValue}")}};
            """;

    private string BuildXmlComment()
        => XmlSummaryContent is null ? "" :
            $"""
            /// <summary>
            /// {XmlSummaryContent}
            /// </summary>
            
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
