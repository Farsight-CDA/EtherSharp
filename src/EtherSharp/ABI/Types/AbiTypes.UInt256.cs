using EtherSharp.ABI.Types.Base;
using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI unsigned 256-bit value.
    /// </summary>
    public class UInt256 : FixedType<Numerics.UInt256>, IPackedEncodeType
    {
        /// <inheritdoc />
        public int PackedSize { get; }

        internal UInt256(Numerics.UInt256 value, int byteLength) : base(value)
        {
            if(byteLength < 9 || byteLength > 32)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(value.BitLength > bitLength)
            {
                throw new ArgumentException($"Value is too large to fit in a {bitLength}-bit unsigned integer", nameof(value));
            }

            PackedSize = byteLength;
        }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        /// <summary>
        /// Encodes an unsigned 256-bit value into ABI or packed form.
        /// </summary>
        public static void EncodeInto(Numerics.UInt256 value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[32];
                BinaryPrimitives.WriteUInt256BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteUInt256BigEndian(buffer, value);
            }
        }

        /// <summary>
        /// Decodes an unsigned 256-bit value from an ABI word.
        /// </summary>
        public static Numerics.UInt256 Decode(ReadOnlySpan<byte> bytes)
            => new Numerics.UInt256(bytes, true);
    }
}
