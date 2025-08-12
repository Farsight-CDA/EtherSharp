using EtherSharp.Generator.SyntaxElements;

namespace EtherSharp.Generator.SourceWriters.Components;
public class AbiTypeWriter(string @namespace)
{
    private readonly Dictionary<SyntaxId, ITypeBuilder> _typeBuilders = [];
    private readonly string _namespace = @namespace;

    public string RegisterTypeBuilder(ITypeBuilder typeBuilder)
    {
        var syntaxId = typeBuilder.GetSyntaxId();

        if(_typeBuilders.TryGetValue(syntaxId, out _))
        {
            return $"{_namespace}.{typeBuilder.TypeName}";
        }

        _typeBuilders.Add(syntaxId, typeBuilder);
        return $"{_namespace}.{typeBuilder.TypeName}";
    }

    public IEnumerable<ITypeBuilder> GetTypeBuilders()
        => _typeBuilders.Values;
}
