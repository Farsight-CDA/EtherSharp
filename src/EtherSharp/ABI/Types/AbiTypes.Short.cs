using EtherSharp.ABI.Types.Base;
using EtherSharp.ABI.Types.Interfaces;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class Short(short value) : FixedType<short>(value), IPackedEncodeType
    {
        public int PackedSize => 2;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(short value, Span<byte> buffer)
        {
            BinaryPrimitives.WriteInt16BigEndian(buffer[(buffer.Length - 2)..], value);

            if(value < 0)
            {
                buffer[..(buffer.Length - 2)].Fill(byte.MaxValue);
            }
        }

        public static short Decode(ReadOnlySpan<byte> bytes)
            => BinaryPrimitives.ReadInt16BigEndian(bytes[(32 - 2)..]);
    }
}
