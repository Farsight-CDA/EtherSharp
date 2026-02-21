using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI <c>uint8</c> value.
    /// </summary>
    public class Byte : FixedType<byte>, IPackedEncodeType
    {
        /// <inheritdoc />
        public int PackedSize => 1;

        internal Byte(byte value) : base(value) { }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a byte value.
        /// </summary>
        public static void EncodeInto(byte value, Span<byte> buffer)
            => buffer[^1] = value;

        /// <summary>
        /// Decodes a byte from an ABI word.
        /// </summary>
        public static byte Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1];
    }
}
