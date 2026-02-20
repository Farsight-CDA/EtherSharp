using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class ULong : FixedType<ulong>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize { get; }

        internal ULong(ulong value, int byteLength) : base(value)
        {
            if(byteLength < 5 || byteLength > 8)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(byteLength));
            }

            int bitLength = byteLength * 8;
            if(byteLength < 8 && value >> bitLength != 0)
            {
                throw new ArgumentException($"Value is too large to fit in a {bitLength}-bit unsigned integer", nameof(value));
            }

            PackedSize = byteLength;
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

        public static void EncodeInto(ulong value, Span<byte> buffer, bool isPacked)
        {
            if(isPacked)
            {
                Span<byte> tempBuffer = stackalloc byte[8];
                BinaryPrimitives.WriteUInt64BigEndian(tempBuffer, value);
                tempBuffer[^buffer.Length..].CopyTo(buffer);
            }
            else
            {
                BinaryPrimitives.WriteUInt64BigEndian(buffer[(buffer.Length - 8)..], value);
            }
        }

        public static ulong Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt64BigEndian(bytes[(32 - 8)..]);
    }
}
