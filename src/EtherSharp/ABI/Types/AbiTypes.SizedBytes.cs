using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class SizedBytes : FixedType<ReadOnlyMemory<byte>>, IPackedEncodeType
    {
        internal SizedBytes(ReadOnlyMemory<byte> value, int byteCount)
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
        void IPackedEncodeType.EncodePacked(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(ReadOnlyMemory<byte> value, Span<byte> buffer)
            => value.Span.CopyTo(buffer[0..value.Length]);
        public static ReadOnlyMemory<byte> Decode(ReadOnlyMemory<byte> bytes, int byteCount)
            => bytes.Slice(0, byteCount);

    }
}
