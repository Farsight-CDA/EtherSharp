using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;
internal static partial class AbiTypes
{
    internal class FixedTuple(IFixedTupleEncoder value) : FixedType<IFixedTupleEncoder>(value)
    {
        public override uint Size => Value.MetadataSize;

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
