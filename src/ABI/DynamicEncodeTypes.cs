using System.Text;

namespace EtherSharp.ABI;

internal interface IDynamicEncodeType : IEncodeType
{
    public void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);
}

internal abstract class DynamicEncodeType<T>(T value) : IDynamicEncodeType
{
    public abstract uint MetadataSize { get; }
    public abstract uint PayloadSize { get; }

    public readonly T Value = value;

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset);

    public class String(string value) : DynamicEncodeType<string>(value)
    {
        public override uint MetadataSize => 32;
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            if(!BitConverter.TryWriteBytes(metadata, payloadOffset))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                metadata.Reverse();
            }

            if(!BitConverter.TryWriteBytes(payload[..32], Value.Length))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload[..32].Reverse();
            }

            if (!Encoding.UTF8.TryGetBytes(Value, payload[32..], out _))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
        }
    }

    public class Bytes(byte[] value) : DynamicEncodeType<byte[]>(value)
    {
        public override uint MetadataSize => 32;
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            byte[] offsetBytes = new byte[32];
            _ = BitConverter.TryWriteBytes(offsetBytes, payloadOffset);
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(offsetBytes);
            }
            offsetBytes.CopyTo(metadata[..32]);

            byte[] lengthBytes = new byte[32];
            _ = BitConverter.TryWriteBytes(lengthBytes, Value.Length);
            if(BitConverter.IsLittleEndian)
            {
                Array.Reverse(lengthBytes);
            }
            lengthBytes.CopyTo(metadata[32..]);

            Value.CopyTo(payload);
        }
    }

    public class AArray(IArrayAbiEncoder value) : DynamicEncodeType<IArrayAbiEncoder>(value)
    {
        public override uint MetadataSize => 32;
        public override uint PayloadSize => Value.PayloadSize + Value.MetadataSize;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
            => Value.WriteToParent(metadata, payload, payloadOffset);
    }
}