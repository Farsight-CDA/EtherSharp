using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class SizedBytes(byte[] value, int byteCount) : FixedType<byte[]>(value), IPackedEncodeType
    {
        public int PackedSize { get; } = byteCount;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, PackedSize, buffer);
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, PackedSize, buffer);

        public static void EncodeInto(byte[] value, int byteCount, Span<byte> buffer)
            => value.CopyTo(buffer[(buffer.Length - byteCount)..]);
        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, int byteCount)
            => bytes[(32 - byteCount)..];

    }
}
