using EtherSharp.Generator.SyntaxElements;

namespace EtherSharp.Generator.SourceWriters.Components;
public class AbiTypeWriter
{
    private readonly Dictionary<SyntaxId, ITypeBuilder> _typeBuilders = [];

    public void RegisterTypeBuilder(ITypeBuilder typeBuilder)
    {
        var syntaxId = typeBuilder.GetSyntaxId();

        if(_typeBuilders.TryGetValue(syntaxId, out _))
        {
            return;
        }

        _typeBuilders.Add(syntaxId, typeBuilder);
    }

    public IEnumerable<ITypeBuilder> GetTypeBuilders()
        => _typeBuilders.Values;
}
