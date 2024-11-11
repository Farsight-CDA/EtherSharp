using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class SByte(sbyte value) : FixedType<sbyte>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(sbyte value, Span<byte> buffer)
        {
            buffer[^1] = (byte) value;

            if(value < 0)
            {
                buffer[..(32 - 1)].Fill(byte.MaxValue);
            }
        }

        public static sbyte Decode(ReadOnlySpan<byte> bytes) => BitConverter.IsLittleEndian ? (sbyte) BinaryPrimitives.ReverseEndianness(bytes[^1]) : (sbyte) bytes[^1];
    }
}
