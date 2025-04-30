using EtherSharp.ABI.Types.Base;
using EtherSharp.ABI.Types.Interfaces;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class Byte(byte value) : FixedType<byte>(value), IPackedEncodeType
    {
        public int PackedSize => 1;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(byte value, Span<byte> buffer)
            => buffer[^1] = value;
        public static byte Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1];

    }
}
