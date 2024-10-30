using EtherSharp.ABI.Encode;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class Struct(uint typeId, IStructAbiEncoder value) : DynamicType<IStructAbiEncoder>(value)
    {
        public override uint PayloadSize => Value.PayloadSize + Value.MetadataSize + 32;

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
            if(!BitConverter.TryWriteBytes(payload[..32], typeId))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload[..32].Reverse();
            }
            Value.Build(payload[32..]);
        }
    }
}
