using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class Bool(bool value) : FixedType<bool>(value), IPackedEncodeType
    {
        public int PackedSize => 1;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(bool value, Span<byte> buffer)
            => buffer[^1] = value ? (byte) 1 : (byte) 0;
        public static bool Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1] == 1;
    }
}
