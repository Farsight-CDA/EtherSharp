using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI unsigned 32-bit value.
    /// </summary>
    public class UInt : FixedType<uint>, IPackedEncodeType
    {
        /// <inheritdoc />
        public int PackedSize { get; }

        internal UInt(uint value, int byteLength) : base(value)
        {
            if(byteLength < 3 || byteLength > 4)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(byteLength < 4 && value >> bitLength != 0)
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
        /// Encodes an unsigned integer into ABI or packed form.
        /// </summary>
        public static void EncodeInto(uint value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[4];
                BinaryPrimitives.WriteUInt32BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteUInt32BigEndian(buffer[(buffer.Length - 4)..], value);
            }
        }

        /// <summary>
        /// Decodes an unsigned integer from an ABI word.
        /// </summary>
        public static uint Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..]);
    }
}