using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class SizedBytes : FixedType<byte[]>, IPackedEncodeType
    {
        public SizedBytes(byte[] value, int byteCount)
            : base(value)
        {
            if(value.Length != byteCount)
            {
                throw new ArgumentException($"Expected array of length {byteCount}, but got {value.Length}");
            }
        }

        public int PackedSize => Value.Length;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(byte[] value, Span<byte> buffer)
            => value.CopyTo(buffer[(buffer.Length - value.Length)..]);
        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, int byteCount)
            => bytes[(32 - byteCount)..];

    }
}
