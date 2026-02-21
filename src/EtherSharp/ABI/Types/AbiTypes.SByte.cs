using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI signed 8-bit value.
    /// </summary>
    public class SByte : FixedType<sbyte>, IPackedEncodeType
    {
        /// <inheritdoc />
        public int PackedSize => 1;

        internal SByte(sbyte value) : base(value) { }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a signed byte value.
        /// </summary>
        public static void EncodeInto(sbyte value, Span<byte> buffer)
        {
            buffer[^1] = (byte) value;

            if(value < 0)
            {
                buffer[..(buffer.Length - 1)].Fill(System.Byte.MaxValue);
            }
        }

        /// <summary>
        /// Decodes a signed byte from an ABI word.
        /// </summary>
        public static sbyte Decode(ReadOnlySpan<byte> bytes)
            => unchecked((sbyte) bytes[^1]);
    }
}