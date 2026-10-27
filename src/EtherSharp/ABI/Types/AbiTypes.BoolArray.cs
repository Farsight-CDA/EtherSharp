using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;
using System.Collections.Immutable;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of booleans.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class BoolArray : DynamicType<ImmutableArray<bool>>
    {
        /// <inheritdoc />
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal BoolArray(ImmutableArray<bool> values)
            : base(values.IsDefault ? [] : values)
        { }

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                Bool.EncodeInto(Value[i], payload.Slice(32 + (i * 32), 32));
            }
        }
    }
}
