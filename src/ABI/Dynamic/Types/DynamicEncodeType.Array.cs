namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicEncodeType<T>
{
    public class Array(IArrayAbiEncoder value) : DynamicEncodeType<IArrayAbiEncoder>(value)
    {
        public override uint MetadataSize => 32;
        public override uint PayloadSize => Value.PayloadSize + Value.MetadataSize;

        public override void Encode(Span<byte> metadata, Span<byte> payload, uint payloadOffset)
            => Value.WriteToParent(metadata, payload, payloadOffset);
    }
}
