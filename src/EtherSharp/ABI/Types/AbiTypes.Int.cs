using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an ABI signed 32-bit value.
    /// </summary>
    public class Int : FixedType<int>, IPackedEncodeType
    {
        /// <inheritdoc />
        public int PackedSize { get; }

        internal Int(int value, int byteLength) : base(value)
        {
            if(byteLength < 3 || byteLength > 4)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(byteLength < 4 && ((value > 0 && value >> (bitLength - 1) != 0) || (value < 0 && value >> (bitLength - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {bitLength}-bit signed integer", nameof(value));
            }

            PackedSize = byteLength;
        }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        /// <summary>
        /// Encodes a signed integer into ABI or packed form.
        /// </summary>
        public static void EncodeInto(int value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[4];
                BinaryPrimitives.WriteInt32BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt32BigEndian(buffer[(buffer.Length - 4)..], value);

                if(value < 0)
                {
                    buffer[..(buffer.Length - 4)].Fill(System.Byte.MaxValue);
                }
            }
        }

        /// <summary>
        /// Decodes a signed integer from an ABI word.
        /// </summary>
        public static int Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadInt32BigEndian(bytes[(32 - 4)..]);
    }
}
