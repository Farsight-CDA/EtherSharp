using System.Text;

namespace EVM.net.ABI;

internal interface IDynamicEncodeType : IEncodeType;

internal abstract class DynamicEncodeType<T>(T value) : IDynamicEncodeType
{
    public readonly T Value = value;

    public abstract int MetadataSize { get; }

    public abstract int PayloadSize { get; }

    public abstract void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset);
    public void Encode(Span<byte> values) => throw new NotImplementedException();

    public class String(string value) : DynamicEncodeType<string>(value)
    {
        public override int MetadataSize => 32;
        public override int PayloadSize => ((Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
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

            byte[] stringBytes = Encoding.UTF8.GetBytes(Value);
            stringBytes.CopyTo(payload);
        }
    }

    public class Bytes(byte[] value) : DynamicEncodeType<byte[]>(value)
    {
        public override int MetadataSize => 32;
        public override int PayloadSize => ((Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
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
        public override int MetadataSize => 32;
        public override int PayloadSize => Value.PayloadSize + Value.MetadataSize;

        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
            => Value.Build(metadata, payload, payloadOffset);
    }
}