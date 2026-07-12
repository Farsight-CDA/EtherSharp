using EtherSharp.ABI.Decode.Interfaces;
using EtherSharp.ABI.Encode.Interfaces;
using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    internal static (ReadOnlyMemory<byte> Data, int Length) DecodeArrayPayload(ReadOnlyMemory<byte> bytes, int metadataOffset)
    {
        if(metadataOffset < 0 || metadataOffset > bytes.Length - 32)
        {
            throw new ArgumentException("ABI array metadata is outside the payload.", nameof(bytes));
        }

        uint encodedPayloadOffset = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metadataOffset + 28)..(metadataOffset + 32)]);
        if(encodedPayloadOffset > Int32.MaxValue || (int) encodedPayloadOffset > bytes.Length - 32)
        {
            throw new ArgumentException("ABI array payload is outside the encoded data.", nameof(bytes));
        }

        int payloadOffset = (int) encodedPayloadOffset;
        uint encodedLength = BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(payloadOffset + 28)..(payloadOffset + 32)]);
        var data = bytes[(payloadOffset + 32)..];

        return encodedLength <= data.Length / 32
            ? (data, (int) encodedLength)
            : throw new ArgumentException("ABI array length exceeds the available payload.", nameof(bytes));
    }

    /// <summary>
    /// Represents a dynamic ABI array.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class Array : DynamicType<IArrayAbiEncoder>
    {
        /// <inheritdoc />
        public override int PayloadSize => Value.MetadataSize + Value.PayloadSize + 32;

        private readonly uint _length;

        internal Array(IArrayAbiEncoder value, uint length) : base(value)
        {
            _length = length;
        }

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], _length);

            if(!Value.TryWriteTo(payload[32..]))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }

        /// <summary>
        /// Decodes an ABI array using the supplied element decoder.
        /// </summary>
        public static T[] Decode<T>(ReadOnlyMemory<byte> bytes, int metaDataOffset, Func<IArrayAbiDecoder, T> decoder)
        {
            var (payload, arrayLength) = DecodeArrayPayload(bytes, metaDataOffset);
            var output = new T[arrayLength];

            var innerDecoder = new AbiDecoder(payload);

            for(int i = 0; i < arrayLength; i++)
            {
                output[i] = decoder.Invoke(innerDecoder);
            }

            return output;
        }
    }
}
