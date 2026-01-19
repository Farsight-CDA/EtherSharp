using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    public class FixedTuple : FixedType<IFixedTupleEncoder>
    {
        public override int Size => Value.MetadataSize;

        internal FixedTuple(IFixedTupleEncoder value) : base(value) { }

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

        public static T Decode<T>(AbiDecoder decoder, Func<IFixedTupleDecoder, T> subDecoder)
            => subDecoder.Invoke(decoder);

    }
}
