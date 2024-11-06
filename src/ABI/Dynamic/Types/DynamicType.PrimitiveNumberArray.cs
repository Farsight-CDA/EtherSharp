using EtherSharp.ABI.Decode;
using EtherSharp.ABI.Fixed;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.ABI.Dynamic;
internal abstract partial class DynamicType<T>
{
    public class PrimitiveNumberArray<TInner> : DynamicType<TInner[]>
        where TInner : INumber<TInner>
    {
        public override uint PayloadSize => (32 * (uint) Value.Length) + 32;

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
                        FixedType<object>.Byte.EncodeInto(us8, slot);
                        break;
                    case sbyte s8:
                        FixedType<object>.SByte.EncodeInto(s8, slot);
                        break;
                    case ushort us16:
                        FixedType<object>.UShort.EncodeInto(us16, slot);
                        break;
                    case short s16:
                        FixedType<object>.Short.EncodeInto(s16, slot);
                        break;
                    case uint us32:
                        FixedType<object>.UInt.EncodeInto(us32, slot);
                        break;
                    case int s32:
                        FixedType<object>.Int.EncodeInto(s32, slot);
                        break;
                    case ulong us64:
                        FixedType<object>.ULong.EncodeInto(us64, slot);
                        break;
                    case long s64:
                        FixedType<object>.Long.EncodeInto(s64, slot);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static TInner[] Decode(ReadOnlyMemory<byte> bytes, uint metaDataOffset, AbiDecoder abiDecoder)
        {
            uint arrayOffest = BitConverter.ToUInt32(bytes[(32 - 4)..].Span);

            if(BitConverter.IsLittleEndian)
            {
                arrayOffest = BinaryPrimitives.ReverseEndianness(arrayOffest);
            }

            long index = arrayOffest - metaDataOffset;
            if(index < 0 || index > int.MaxValue)
            {
                throw new IndexOutOfRangeException("Index out of range");
            }

            uint leng = BitConverter.ToUInt32(bytes[(int) (index + 32 - 4)..(int) (index + 32)].Span);

            if(BitConverter.IsLittleEndian)
            {
                leng = BinaryPrimitives.ReverseEndianness(leng);
            }

            var data = bytes[(int) (index + 32)..];

            switch(typeof(TInner))
            {
                case Type us8 when us8 == typeof(byte):
                {
                    byte[] arr = new byte[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.Byte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s8 when s8 == typeof(sbyte):
                {
                    sbyte[] arr = new sbyte[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.SByte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us16 when us16 == typeof(ushort):
                {
                    ushort[] arr = new ushort[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.UShort.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s16 when s16 == typeof(short):
                {
                    short[] arr = new short[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.Short.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us32 when us32 == typeof(uint):
                {
                    uint[] arr = new uint[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.UInt.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s32 when s32 == typeof(int):
                {
                    int[] arr = new int[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.Int.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                };
                case Type us64 when us64 == typeof(ulong):
                {
                    ulong[] arr = new ulong[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.ULong.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s64 when s64 == typeof(long):
                {
                    long[] arr = new long[leng];
                    for(int i = 0; i < leng; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = FixedType<object>.Long.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                default:
                    throw new ArgumentException($"Expected primitive number type, got {typeof(TInner)}");

            }
        }
    }
}