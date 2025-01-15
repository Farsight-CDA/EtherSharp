using EtherSharp.Types;

namespace EtherSharp.StateOverride;
public sealed class MappingStateOverrideBuilder(Action<Span<byte>, byte[]> entryAction)
{
    private readonly Action<Span<byte>, byte[]> _entryAction = entryAction;

    public MappingStateOverrideBuilder AddValue(Address address, byte[] value)
    {
        Span<byte> addressBuffer = stackalloc byte[32];
        address.Bytes.CopyTo(addressBuffer[12..]);
        _entryAction(addressBuffer, value);
        return this;
    }
}
