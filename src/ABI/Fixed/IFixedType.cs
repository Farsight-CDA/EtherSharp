using EtherSharp.ABI.Encode;

namespace EtherSharp.ABI.Fixed;
internal interface IFixedType : IEncodeType
{
    public void Encode(Span<byte> buffer);
}
