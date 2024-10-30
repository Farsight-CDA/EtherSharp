using EtherSharp.ABI.Encode;
using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class EncodeTypeArray<TInner>(TInner[] value) : DynamicType<TInner[]>(value)
        where TInner : IEncodeType
    {
        public override uint PayloadSize => (uint) Value.Sum(x => x is IDynamicType dynType ? dynType.PayloadSize + 32 : 32) + 32;

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

            if(!BitConverter.TryWriteBytes(payload.Slice(28, 4), (uint) Value.Length))
            {
                throw new InvalidOperationException("Failed to write bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                payload.Slice(28, 4).Reverse();
            }

            uint localPayloadOffset = 32 * (uint) Value.Length;
            for(int i = 0; i < Value.Length; i++) 
            {
                int localMetadataOffset = 32 + (32 * i);

                switch(Value[i])
                {
                    case IDynamicType dynType:
                        dynType.Encode(
                            payload.Slice(localMetadataOffset, 32), 
                            payload.Slice(32 + (int) localPayloadOffset, (int) dynType.PayloadSize), 
                            localPayloadOffset
                        );
                        localPayloadOffset += dynType.PayloadSize;
                        break;
                    case IFixedType fixType:
                        fixType.Encode(payload.Slice(localMetadataOffset, 32));
                        break;
                }
            }
        }
    }
}
