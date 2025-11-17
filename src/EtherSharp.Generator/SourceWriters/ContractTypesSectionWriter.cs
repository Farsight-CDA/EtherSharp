using EtherSharp.Generator.SyntaxElements;

namespace EtherSharp.Generator.SourceWriters;

public class ContractTypesSectionWriter(string @namespace, string contractInterfaceName)
{
    private readonly Dictionary<SyntaxId, ITypeBuilder> _typeBuilders = [];
    private readonly string _namespace = @namespace;

    public string RegisterTypeBuilder(ITypeBuilder typeBuilder)
    {
        var syntaxId = typeBuilder.GetSyntaxId();

        if(_typeBuilders.TryGetValue(syntaxId, out _))
        {
            return $"{_namespace}.{contractInterfaceName}.Types.{typeBuilder.TypeName}";
        }

        _typeBuilders.Add(syntaxId, typeBuilder);
        return $"{_namespace}.{contractInterfaceName}.Types.{typeBuilder.TypeName}";
    }

    public IEnumerable<ITypeBuilder> GetTypeBuilders()
        => _typeBuilders.Values;

    public void GenerateContractTypesSection(InterfaceBuilder interfaceBuilder)
    {
        var typeSection = new ClassBuilder("Types")
            .WithIsStatic(true)
            .WithVisibility(ClassVisibility.Public);

        foreach(var type in _typeBuilders)
        {
            typeSection.AddInnerType(type.Value);
        }

        interfaceBuilder.AddInnerType(typeSection);
    }
}
