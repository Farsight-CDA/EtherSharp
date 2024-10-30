namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Byte(byte value) : FixedType<byte>(value)
    {
        public override void Encode(Span<byte> values) => values[^1] = Value;
    }
}
