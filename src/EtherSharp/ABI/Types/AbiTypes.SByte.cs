using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;
public static partial class AbiTypes
{
    public class SByte : FixedType<sbyte>, IPackedEncodeType
    {
        /// <inheritdoc/>
        public int PackedSize => 1;

        internal SByte(sbyte value) : base(value) { }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
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
