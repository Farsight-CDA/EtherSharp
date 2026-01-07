using EtherSharp.ABI.Types.Base;
using EtherSharp.Numerics;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class Int256 : FixedType<Numerics.Int256>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize { get; }

        internal Int256(Numerics.Int256 value, int byteLength) : base(value)
        {
            if(byteLength < 9 || byteLength > 32)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(byteLength < 32 && ((value > 0 && value >> (bitLength - 1) != 0) || (value < 0 && value >> (bitLength - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {bitLength}-bit signed integer", nameof(value));
            }

            PackedSize = byteLength;
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        public static void EncodeInto(Numerics.Int256 value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[32];
                BinaryPrimitives.WriteInt256BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteInt256BigEndian(buffer, value);
            }
        }

        public static Numerics.Int256 Decode(ReadOnlySpan<byte> bytes)
            => new Numerics.Int256(bytes, true);
    }
}
