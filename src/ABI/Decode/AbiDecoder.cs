using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI.Decode;

public partial class AbiDecoder
{
    private readonly Memory<byte> _bytes;

    public AbiDecoder(Memory<byte> bytes)
    {
        _bytes = bytes;
    }

    public AbiDecoder Int8(out int value)
    {
        value = FixedType<string>.Short.Decode(_bytes, 8);
        return this;
    }
}