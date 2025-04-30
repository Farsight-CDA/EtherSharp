namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType
{
    internal class Bool(bool value) : FixedType<bool>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public static void EncodeInto(bool value, Span<byte> buffer)
            => buffer[^1] = value ? (byte) 1 : (byte) 0;
        public static bool Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1] == 1;
    }
}
