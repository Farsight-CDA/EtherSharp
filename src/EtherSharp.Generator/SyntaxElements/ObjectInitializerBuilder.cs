using Microsoft.CodeAnalysis;
using System.Text;

namespace EtherSharp.Generator.SyntaxElements;

public class InitializerArgument(string targetPropertyName, string sourceExpression, bool isReadonlyList)
{
    public string TargetPropertyName { get; } = targetPropertyName;
    public string SourceExpression { get; } = sourceExpression;

    public bool IsReadonlyList { get; } = isReadonlyList;
}
public class ObjectInitializerBuilder
{
    private readonly List<InitializerArgument> _arguments;

    public ObjectInitializerBuilder()
    {
        _arguments = [];
    }

    public ObjectInitializerBuilder AddArgument(IPropertySymbol property, string sourceExpression)
    {
        _arguments.Add(new InitializerArgument(property.Name, sourceExpression, IsReadonlyList(property)));
        return this;
    }

    public ObjectInitializerBuilder AddArgument(string propertyName, string sourceExpression, bool isReadonlyList = false)
    {
        _arguments.Add(new InitializerArgument(propertyName, sourceExpression, isReadonlyList));
        return this;
    }

    public string ToInlineInitializer()
    {
        var sb = new StringBuilder();

        if(_arguments.Count == 0)
        {
            return String.Empty;
        }

        _ = sb.AppendLine("{");

        foreach(var argument in _arguments)
        {
            if(argument.IsReadonlyList)
            {
                throw new InvalidOperationException("Tried to use inline initializer for readonly list property");
            }

            _ = sb.AppendLine($"{argument.TargetPropertyName} = {argument.SourceExpression},");
        }

        _ = sb.AppendLine("}");

        return sb.ToString();
    }

    public string ToMultilineInitializer(string variableName)
    {
        var sb = new StringBuilder();

        if(_arguments.Count == 0)
        {
            return String.Empty;
        }

        _ = sb.AppendLine("{");

        foreach(var argument in _arguments)
        {
            if(argument.IsReadonlyList)
            {
                continue;
            }

            _ = sb.AppendLine($"{argument.TargetPropertyName} = {argument.SourceExpression},");
        }

        _ = sb.AppendLine("};");

        foreach(var argument in _arguments)
        {
            if(!argument.IsReadonlyList)
            {
                continue;
            }

            _ = sb.AppendLine($"{variableName}.{argument.TargetPropertyName}.AddRange({argument.SourceExpression});");
        }

        return sb.ToString();
    }

    private static bool IsReadonlyList(IPropertySymbol property)
        => property.Type.Name switch
        {
            "RepeatedField" => true,
            _ => false,
        };

}

