using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI signed 64-bit value.
    /// </summary>
    public class Long : FixedType<long>, IPackedEncodeType
    {
        /// <summary>
        /// Gets the packed encoded size in bytes.
        /// </summary>
        public int PackedSize { get; }

        internal Long(long value, int byteLength) : base(value)
        {
            if(byteLength < 5 || byteLength > 8)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(byteLength < 8 && ((value > 0 && value >> (bitLength - 1) != 0) || (value < 0 && value >> (bitLength - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {bitLength}-bit signed integer", nameof(value));
            }

            PackedSize = byteLength;
        }

        /// <summary>
        /// Writes the value into the target buffer.
        /// </summary>
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        /// <summary>
        /// Encodes a signed long into ABI or packed form.
        /// </summary>
        public static void EncodeInto(long value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[8];
                BinaryPrimitives.WriteInt64BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt64BigEndian(buffer[(32 - 8)..], value);

                if(value < 0)
                {
                    buffer[..(32 - 8)].Fill(System.Byte.MaxValue);
                }
            }
        }

        /// <summary>
        /// Decodes a signed long from an ABI word.
        /// </summary>
        public static long Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadInt64BigEndian(bytes[(32 - 8)..]);
    }
}
