using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI fixed-size byte array value.
    /// </summary>
    public class SizedBytes : FixedType<ReadOnlyMemory<byte>>, IPackedEncodeType
    {
        internal SizedBytes(ReadOnlyMemory<byte> value, int byteCount)
            : base(value)
        {
            if(value.Length != byteCount)
            {
                throw new ArgumentException($"Expected array of length {byteCount}, but got {value.Length}");
            }
        }

        /// <summary>
        /// Gets the packed encoded size in bytes.
        /// </summary>
        public int PackedSize => Value.Length;

        /// <summary>
        /// Writes the fixed bytes into the target buffer.
        /// </summary>
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a fixed-size byte array value.
        /// </summary>
        public static void EncodeInto(ReadOnlyMemory<byte> value, Span<byte> buffer)
            => value.Span.CopyTo(buffer[0..value.Length]);

        /// <summary>
        /// Decodes a fixed-size byte array value.
        /// </summary>
        public static ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> bytes, int byteCount)
            => bytes.Slice(0, byteCount);
    }
}
