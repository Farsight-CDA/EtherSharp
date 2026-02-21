using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a fixed-size ABI tuple.
    /// </summary>
    public class FixedTuple : FixedType<IFixedTupleEncoder>
    {
        /// <inheritdoc />
        public override int Size => Value.MetadataSize;

        internal FixedTuple(IFixedTupleEncoder value) : base(value) { }

        /// <inheritdoc />
        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        /// <summary>
        /// Encodes a fixed tuple value.
        /// </summary>
        public static void EncodeInto(IFixedTupleEncoder value, Span<byte> buffer)
        {
            if(value.PayloadSize != 0)
            {
                throw new InvalidOperationException("Tried to encode a dynamic value as a fixed tuple");
            }

            value.TryWriteTo(buffer);
        }

        /// <summary>
        /// Decodes a fixed tuple using the supplied decoder.
        /// </summary>
        public static T Decode<T>(AbiDecoder decoder, Func<IFixedTupleDecoder, T> subDecoder)
            => subDecoder.Invoke(decoder);
    }
}