using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class UInt : FixedType<uint>, IPackedEncodeType
    {
        public int PackedSize { get; }

        public UInt(uint value, int byteLength) : base(value)
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

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer, false);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer, true);

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

        public static uint Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadUInt32BigEndian(bytes[(32 - 4)..]);
    }
}
