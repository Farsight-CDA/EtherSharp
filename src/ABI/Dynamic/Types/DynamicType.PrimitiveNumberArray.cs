using System.Numerics;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class PrimitiveNumberArray<TInner> : DynamicType<TInner[]>
        where TInner : INumber<TInner>
    {
        public override uint PayloadSize => 32 * (uint) Value.Length + 32;

        public PrimitiveNumberArray(TInner[] value, int length)
            : base(value)
        {
            for(int i = 0; i < Value.Length; i++)
            {
                if(Value[i] switch
                {
                    byte => false,
                    sbyte => false,
                    ushort => false,
                    short => false,
                    uint us32 => length != 32 && us32 >> length != 0,
                    int s32 => length != 32 && ((s32 > 0 && s32 >> (length - 1) != 0) || (s32 < 0 && s32 >> (length - 1) != -1)),
                    ulong us64 => length != 64 && us64 >> length != 0,
                    long s64 => length != 64 && ((s64 > 0 && s64 >> (length - 1) != 0) || (s64 < 0 && s64 >> (length - 1) != -1)),
                    _ => throw new ArgumentException($"Expected primitive number type, got {Value[i].GetType()}")
                })
                {
                    throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
                }
            }
        }

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

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                switch(Value[i])
                {
                    case byte us8:
                        slot[^1] = us8;
                        break;
                    case sbyte s8:
                        slot[^1] = (byte) s8;
                        break;
                    case ushort us16:
                        BitConverter.TryWriteBytes(slot[30..], us16);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[30..].Reverse();
                        }
                        break;
                    case short s16:
                        BitConverter.TryWriteBytes(slot[30..], s16);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[30..].Reverse();
                        }
                        break;
                    case uint us32:
                        BitConverter.TryWriteBytes(slot[28..], us32);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[28..].Reverse();
                        }
                        break;
                    case int s32:
                        BitConverter.TryWriteBytes(slot[28..], s32);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[28..].Reverse();
                        }
                        break;
                    case ulong us64:
                        BitConverter.TryWriteBytes(slot[24..], us64);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[24..].Reverse();
                        }
                        break;
                    case long s64:
                        BitConverter.TryWriteBytes(slot[24..], s64);
                        if(BitConverter.IsLittleEndian)
                        {
                            slot[24..].Reverse();
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }
    }
}