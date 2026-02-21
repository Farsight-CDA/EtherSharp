using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of primitive numeric values.
    /// </summary>
    public class SizedNumberArray<TInner> : DynamicType<TInner[]>
        where TInner : INumber<TInner>
    {
        /// <summary>
        /// Gets the encoded payload size in bytes.
        /// </summary>
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal SizedNumberArray(TInner[] value, int length)
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

        /// <summary>
        /// Encodes array metadata and payload.
        /// </summary>
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            for(int i = 0; i < Value.Length; i++)
            {
                var slot = payload.Slice(32 + (i * 32), 32);
                switch(Value[i])
                {
                    case byte us8:
                        AbiTypes.Byte.EncodeInto(us8, slot);
                        break;
                    case sbyte s8:
                        AbiTypes.SByte.EncodeInto(s8, slot);
                        break;
                    case ushort us16:
                        AbiTypes.UShort.EncodeInto(us16, slot);
                        break;
                    case short s16:
                        AbiTypes.Short.EncodeInto(s16, slot);
                        break;
                    case uint us32:
                        AbiTypes.UInt.EncodeInto(us32, slot, false);
                        break;
                    case int s32:
                        AbiTypes.Int.EncodeInto(s32, slot, false);
                        break;
                    case ulong us64:
                        AbiTypes.ULong.EncodeInto(us64, slot, false);
                        break;
                    case long s64:
                        AbiTypes.Long.EncodeInto(s64, slot, false);
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// Decodes an array of primitive numeric values.
        /// </summary>
        public static TInner[] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset)
        {
            int arrayOffest = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(metaDataOffset + 28)..(metaDataOffset + 32)]);
            int length = (int) BinaryPrimitives.ReadUInt32BigEndian(bytes.Span[(arrayOffest + 28)..(arrayOffest + 32)]);
            var data = bytes[(arrayOffest + 32)..];

            switch(typeof(TInner))
            {
                case Type us8 when us8 == typeof(byte):
                {
                    byte[] arr = new byte[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.Byte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s8 when s8 == typeof(sbyte):
                {
                    sbyte[] arr = new sbyte[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.SByte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us16 when us16 == typeof(ushort):
                {
                    ushort[] arr = new ushort[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.UShort.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s16 when s16 == typeof(short):
                {
                    short[] arr = new short[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.Short.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us32 when us32 == typeof(uint):
                {
                    uint[] arr = new uint[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.UInt.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s32 when s32 == typeof(int):
                {
                    int[] arr = new int[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.Int.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                ;
                case Type us64 when us64 == typeof(ulong):
                {
                    ulong[] arr = new ulong[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.ULong.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s64 when s64 == typeof(long):
                {
                    long[] arr = new long[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = AbiTypes.Long.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                default:
                    throw new ArgumentException($"Expected primitive number type, got {typeof(TInner)}");

            }
        }
    }
}
