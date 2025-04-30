using EtherSharp.ABI.Types.Base;
using EtherSharp.ABI.Types.Interfaces;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class SizedBytes(byte[] value, int byteCount) : FixedType<byte[]>(value), IPackedEncodeType
    {
        private readonly int _byteCount = byteCount;

        public int PackedSize => _byteCount;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, _byteCount, buffer); 
        public void EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, _byteCount, buffer);

        public static void EncodeInto(byte[] value, int byteCount, Span<byte> buffer)
            => value.CopyTo(buffer[(buffer.Length - byteCount)..]);
        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, int byteCount)
            => bytes[(32 - byteCount)..];

    }
}
