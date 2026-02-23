using System.Text;

namespace EtherSharp.Generator.SyntaxElements;

public enum InterfaceVisibility
{
    None,
    Public,
    Internal
}
public class InterfaceBuilder(string name)
{
    private readonly List<FunctionBuilder> _functions = [];
    private readonly List<string> _baseInterfaces = [];
    private readonly List<ITypeBuilder> _innerTypes = [];
    private readonly List<string> _rawContents = [];

    private readonly string _name = name;
    private InterfaceVisibility _visibility = InterfaceVisibility.None;
    private bool _isPartial;

    public InterfaceBuilder AddFunction(FunctionBuilder function)
    {
        _functions.Add(function);
        return this;
    }

    public InterfaceBuilder AddFunctions(IEnumerable<FunctionBuilder> functions)
    {
        _functions.AddRange(functions);
        return this;
    }

    public InterfaceBuilder AddBaseInterface(string name)
    {
        _baseInterfaces.Add(name);
        return this;
    }

    public InterfaceBuilder AddInnerType(ITypeBuilder typeBuilder)
    {
        _innerTypes.Add(typeBuilder);
        return this;
    }

    public InterfaceBuilder AddRawContent(string rawContent)
    {
        _rawContents.Add(rawContent);
        return this;
    }

    public InterfaceBuilder AddBaseTypes(IEnumerable<BaseType> baseTypes)
    {
        foreach(var baseType in baseTypes)
        {
            if(!baseType.IsInterface)
            {
                continue;
            }

            _ = AddBaseInterface(baseType.Name);
        }

        return this;
    }

    public InterfaceBuilder WithVisibility(InterfaceVisibility visibility)
    {
        _visibility = visibility;
        return this;
    }

    public InterfaceBuilder WithIsPartial(bool isPartial)
    {
        _isPartial = isPartial;
        return this;
    }

    public string Build()
    {
        var bodySb = new StringBuilder();
        var baseInterfacesSb = new StringBuilder();

        foreach(var function in _functions)
        {
            _ = bodySb.AppendLine(function.BuildInterfaceDefinition());
        }
        foreach(var innerType in _innerTypes)
        {
            _ = bodySb.AppendLine(innerType.Build());
        }
        foreach(string rawContent in _rawContents)
        {
            _ = bodySb.AppendLine(rawContent);
        }

        for(int i = 0; i < _baseInterfaces.Count; i++)
        {
            if(i == 0)
            {
                _ = baseInterfacesSb.Append(" : ");
            }

            _ = baseInterfacesSb.Append(_baseInterfaces[i]);

            if(i < _baseInterfaces.Count - 1)
            {
                _ = baseInterfacesSb.Append(", ");
            }
        }

        return
            $$"""
            {{PrintVisibility(_visibility)}}{{(_isPartial ? "partial " : "")}}interface {{_name}} {{baseInterfacesSb}} 
            {
                {{bodySb}}
            }
            """;
    }

    private static string PrintVisibility(InterfaceVisibility visibility)
        => visibility switch
        {
            InterfaceVisibility.None => "",
            InterfaceVisibility.Public => "public ",
            InterfaceVisibility.Internal => "internal ",
            _ => throw new NotSupportedException()
        };
}
