using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;
using System.Collections.Immutable;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents an array of dynamic encode types.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class DynamicTypeArray<TInner> : DynamicType<ImmutableArray<TInner>>
        where TInner : IDynamicType
    {
        private readonly int _payloadSize;

        /// <inheritdoc />
        public override int PayloadSize => _payloadSize;

        internal DynamicTypeArray(ImmutableArray<TInner> value) : base(value.IsDefault ? [] : value)
        {
            int total = 32 + (Value.Length * 32);
            foreach(var item in Value)
            {
                total += item.PayloadSize;
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

                var item = Value[i];
                item.Encode(
                    payload.Slice(localMetadataOffset, 32),
                    payload.Slice(32 + localPayloadOffset, item.PayloadSize),
                    localPayloadOffset
                );
                localPayloadOffset += item.PayloadSize;
            }
        }
    }
}
