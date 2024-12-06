namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Bytes(byte[] value, int byteCount) : FixedType<byte[]>(value)
    {
        private readonly int _byteCount = byteCount;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, _byteCount, buffer);

        public static void EncodeInto(byte[] value, int byteCount, Span<byte> buffer)
            => value.CopyTo(buffer[(32 - byteCount)..]);
        public static ReadOnlySpan<byte> Decode(ReadOnlySpan<byte> bytes, int byteCount)
            => bytes[(32 - byteCount)..];
    }
}
