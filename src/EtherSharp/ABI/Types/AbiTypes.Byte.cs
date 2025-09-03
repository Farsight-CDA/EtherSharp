using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class Byte : FixedType<byte>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 1;

        internal Byte(byte value) : base(value) { }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(byte value, Span<byte> buffer)
            => buffer[^1] = value;
        public static byte Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1];

    }
}
