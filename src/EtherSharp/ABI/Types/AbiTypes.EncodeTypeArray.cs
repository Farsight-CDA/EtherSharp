using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an array of encode types.
    /// </summary>
    public class EncodeTypeArray<TInner> : DynamicType<TInner[]>
        where TInner : IEncodeType
    {
        private readonly int _payloadSize;

        /// <inheritdoc />
        public override int PayloadSize => _payloadSize;

        internal EncodeTypeArray(TInner[] value) : base(value)
        {
            int total = 32 + (value.Length * 32);
            foreach(var item in value)
            {
                if(item is IDynamicType dynType)
                {
                    total += dynType.PayloadSize;
                }
            }

            _payloadSize = total;
        }

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            int localPayloadOffset = 32 * Value.Length;
            for(int i = 0; i < Value.Length; i++)
            {
                int localMetadataOffset = 32 + (32 * i);

                switch(Value[i])
                {
                    case IDynamicType dynType:
                        dynType.Encode(
                            payload.Slice(localMetadataOffset, 32),
                            payload.Slice(32 + localPayloadOffset, dynType.PayloadSize),
                            localPayloadOffset
                        );
                        localPayloadOffset += dynType.PayloadSize;
                        break;
                    case IFixedType fixType:
                        fixType.Encode(payload.Slice(localMetadataOffset, fixType.Size));
                        break;
                }
            }
        }
    }
}