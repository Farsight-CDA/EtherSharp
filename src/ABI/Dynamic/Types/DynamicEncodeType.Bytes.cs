namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicEncodeType<T>
{
    public class Bytes(byte[] value) : DynamicEncodeType<byte[]>(value)
    {
        public override uint PayloadSize => (((uint) Value.Length + 31) / 32 * 32) + 32;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
        {
            byte[] offsetBytes = new byte[32];
            _ = BitConverter.TryWriteBytes(offsetBytes, payloadOffset);
            if(BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(offsetBytes);
            }
            offsetBytes.CopyTo(metadata[..32]);

            byte[] lengthBytes = new byte[32];
            _ = BitConverter.TryWriteBytes(lengthBytes, Value.Length);
            if(BitConverter.IsLittleEndian)
            {
                System.Array.Reverse(lengthBytes);
            }
            lengthBytes.CopyTo(metadata[32..]);

            Value.CopyTo(payload);
        }
    }
}
