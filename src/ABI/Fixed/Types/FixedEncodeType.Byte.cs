namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedEncodeType<T>
{
    internal class Byte(byte value) : FixedEncodeType<byte>(value)
    {
        public override void Encode(Span<byte> values) => values[^1] = Value;
    }
}
