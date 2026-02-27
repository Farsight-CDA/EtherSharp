using EtherSharp.ABI.Types.Base;
using EtherSharp.Types;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI fixed-size byte array value.
    /// </summary>
    public class SizedBytes<TBytes> : FixedType<TBytes>, IPackedEncodeType
        where TBytes : struct, IFixedBytes<TBytes>
    {
        internal SizedBytes(TBytes value)
            : base(value)
        { }

        /// <inheritdoc />
        public int PackedSize => TBytes.BYTE_LENGTH;

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a fixed-size byte array value.
        /// </summary>
        public static void EncodeInto(TBytes value, Span<byte> buffer)
            => value.Bytes.CopyTo(buffer[..TBytes.BYTE_LENGTH]);

        /// <summary>
        /// Decodes a fixed-size byte array value.
        /// </summary>
        public static TBytes Decode(ReadOnlyMemory<byte> bytes)
            => TBytes.FromBytes(bytes.Span[..TBytes.BYTE_LENGTH]);
    }
}
