using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType
{
    internal class Short(short value) : FixedType<short>(value)
    {
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(short value, Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer[(32 - 2)..], value);

            if(value < 0)
            {
                buffer[..(32 - 2)].Fill(byte.MaxValue);
            }
        }

        public static short Decode(ReadOnlySpan<byte> bytes) 
            => BinaryPrimitives.ReadInt16BigEndian(bytes[(32 - 2)..]);
    }
}
