using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class SByte(sbyte value) : FixedType<sbyte>(value), IPackedEncodeType
    {
        public int PackedSize => 1;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(sbyte value, Span<byte> buffer)
        {
            buffer[^1] = (byte) value;

            if(value < 0)
            {
                buffer[..(buffer.Length - 1)].Fill(byte.MaxValue);
            }
        }

        public static sbyte Decode(ReadOnlySpan<byte> bytes)
            => BitConverter.IsLittleEndian
                ? (sbyte) BinaryPrimitives.ReverseEndianness(bytes[^1]) : (sbyte) bytes[^1];

    }
}
