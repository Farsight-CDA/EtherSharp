using EtherSharp.Generator.Util;
using System.Globalization;
using System.Text;

namespace EtherSharp.Generator.SyntaxElements;
public enum PropertyVisibility
{
    Public,
    Private,
    Internal
}
public enum SetterVisibility
{
    None,
    Public,
    Private,
    Init
}
public class PropertyBuilder(string type, string name) : ISyntaxBuilder
{
    public string Type { get; private set; } = type;
    public string Name { get; private set; } = name;
    public string? DefaultValue { get; private set; }

    private PropertyVisibility _visibility = PropertyVisibility.Public;
    private SetterVisibility _setterVisibility = SetterVisibility.Public;

    private bool _isRequired;

    private string? _jsonPropertyName;
    private string? _summaryComment;

    public PropertyBuilder WithVisibility(PropertyVisibility visibility)
    {
        _visibility = visibility;
        return this;
    }

    public PropertyBuilder WithSetterVisibility(SetterVisibility setterVisibility)
    {
        _setterVisibility = setterVisibility;
        return this;
    }

    public PropertyBuilder WithIsRequired(bool isRequired = true)
    {
        _isRequired = isRequired;
        return this;
    }

    public PropertyBuilder WithSummaryComment(string summaryComment)
    {
        _summaryComment = summaryComment;
        return this;
    }

    public PropertyBuilder WithJsonPropertyName(string jsonPropertyName)
    {
        _jsonPropertyName = jsonPropertyName;
        return this;
    }

    public PropertyBuilder WithDefaultValue(string? defaultValue)
    {
        DefaultValue = defaultValue;
        return this;
    }

    public string Build()
    {
        var headerSb = new StringBuilder();

        if(_summaryComment is not null)
        {
            headerSb.AppendLine(CommentUtils.MakeSummaryComment(_summaryComment));
        }
        if(_jsonPropertyName is not null)
        {
            headerSb.AppendLine($"[System.Text.Json.Serialization.JsonPropertyName(\"{_jsonPropertyName}\")]");
        }

        return $$"""
            {{headerSb}} {{_visibility.ToString().ToLower(CultureInfo.InvariantCulture)}} {{(_isRequired ? "required" : "")}} {{Type}} {{Name}} { get; {{GetSetter()}} } {{(DefaultValue is not null ? $"= {DefaultValue};" : "")}}
            """;
    }

    private string GetSetter()
        => _setterVisibility switch
        {
            SetterVisibility.None => "",
            SetterVisibility.Init => "init;",
            SetterVisibility.Public =>
                _visibility == PropertyVisibility.Public
                    ? "set;"
                    : throw new NotSupportedException("Setter visibility must be more restrictive than property visibility"),
            SetterVisibility.Private =>
                _visibility == PropertyVisibility.Private
                    ? "set;"
                    : "private set;",
            _ => throw new NotSupportedException()
        };

    public override int GetHashCode()
        => System.HashCode.Combine(
            Type,
            Name
        );
    public SyntaxId GetSyntaxId()
    {
        int hashCode = HashCode.Combine(
                Type,
                Name,
                _visibility,
                _setterVisibility,
                _isRequired,
                _jsonPropertyName
        );

        return new SyntaxId(hashCode);
    }
}
