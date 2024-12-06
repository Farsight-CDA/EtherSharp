using EtherSharp.ABI.Encode.Interfaces;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Tuple(IFixedTupleEncoder value) : FixedType<IFixedTupleEncoder>(value)
    {
        public override uint MetadataSize => Value.MetadataSize;

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);
        public static void EncodeInto(IFixedTupleEncoder value, Span<byte> buffer)
        {
            if(value.PayloadSize != 0)
            {
                throw new InvalidOperationException("Tried to encode a dynamic value as a fixed tuple");
            }

            value.TryWritoTo(buffer);
        }

        public static bool Decode(ReadOnlySpan<byte> bytes)
            => bytes[^1] == 1;
    }
}
