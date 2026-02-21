using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI signed 16-bit value.
    /// </summary>
    public class Short : FixedType<short>, IPackedEncodeType
    {
        /// <summary>
        /// Gets the packed encoded size in bytes.
        /// </summary>
        public int PackedSize => 2;

        internal Short(short value) : base(value) { }

        /// <summary>
        /// Writes the value into the target buffer.
        /// </summary>
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a signed short value.
        /// </summary>
        public static void EncodeInto(short value, Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer[(buffer.Length - 2)..], value);

            if(value < 0)
            {
                buffer[..(buffer.Length - 2)].Fill(System.Byte.MaxValue);
            }
        }

        /// <summary>
        /// Decodes a signed short from an ABI word.
        /// </summary>
        public static short Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadInt16BigEndian(bytes[(32 - 2)..]);
    }
}
