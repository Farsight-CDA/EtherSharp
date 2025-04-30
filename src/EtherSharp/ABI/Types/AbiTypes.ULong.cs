using EtherSharp.ABI.Types.Base;
using EtherSharp.ABI.Types.Interfaces;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class ULong : FixedType<ulong>, IPackedEncodeType
    {
        private readonly int _byteLength;
        public int PackedSize => _byteLength;

        public ULong(ulong value, int byteLength) : base(value)
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

            _byteLength = byteLength;
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        public void EncodePacked(Span<byte> buffer)
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
