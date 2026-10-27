using EtherSharp.ABI.Types.Base;
using System.Buffers.Binary;
using System.Collections.Immutable;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace EtherSharp.ABI.Types;

public static partial class AbiTypes
{
    /// <summary>
    /// Represents a dynamic array of primitive numeric values.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class SizedNumberArray<TInner> : DynamicType<ImmutableArray<TInner>>
        where TInner : INumber<TInner>
    {
        /// <inheritdoc />
        public override int PayloadSize => (32 * Value.Length) + 32;

        internal SizedNumberArray(ImmutableArray<TInner> value, int length)
            : base(value.IsDefault ? [] : value)
        {
            switch(default(TInner))
            {
                case byte or sbyte or ushort or short:
                case uint or int when length == 32:
                case ulong or long when length == 64:
                    return;
            }

            for(int i = 0; i < Value.Length; i++)
            {
                if(Value[i] switch
                {
                    uint us32 => us32 >> length != 0,
                    int s32 => (s32 > 0 && s32 >> (length - 1) != 0) || (s32 < 0 && s32 >> (length - 1) != -1),
                    ulong us64 => us64 >> length != 0,
                    long s64 => (s64 > 0 && s64 >> (length - 1) != 0) || (s64 < 0 && s64 >> (length - 1) != -1),
                    _ => throw new ArgumentException($"Expected primitive number type, got {Value[i].GetType()}")
                })
                {
                    throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
                }
            }
        }

        /// <inheritdoc />
        public override void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(metadata[28..32], (uint) payloadOffset);
            BinaryPrimitives.WriteUInt32BigEndian(payload[28..32], (uint) Value.Length);

            var value = ImmutableCollectionsMarshal.AsArray(Value)!;
            switch(typeof(TInner))
            {
                case Type us8 when us8 == typeof(byte):
                {
                    byte[] values = Unsafe.As<TInner[], byte[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        Byte.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32));
                    }

                    break;
                }
                case Type s8 when s8 == typeof(sbyte):
                {
                    sbyte[] values = Unsafe.As<TInner[], sbyte[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        SByte.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32));
                    }

                    break;
                }
                case Type us16 when us16 == typeof(ushort):
                {
                    ushort[] values = Unsafe.As<TInner[], ushort[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        UShort.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32));
                    }

                    break;
                }
                case Type s16 when s16 == typeof(short):
                {
                    short[] values = Unsafe.As<TInner[], short[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        Short.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32));
                    }

                    break;
                }
                case Type us32 when us32 == typeof(uint):
                {
                    uint[] values = Unsafe.As<TInner[], uint[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        UInt.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32), false);
                    }

                    break;
                }
                case Type s32 when s32 == typeof(int):
                {
                    int[] values = Unsafe.As<TInner[], int[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        Int.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32), false);
                    }

                    break;
                }
                case Type us64 when us64 == typeof(ulong):
                {
                    ulong[] values = Unsafe.As<TInner[], ulong[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        ULong.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32), false);
                    }

                    break;
                }
                case Type s64 when s64 == typeof(long):
                {
                    long[] values = Unsafe.As<TInner[], long[]>(ref value);
                    for(int i = 0; i < values.Length; i++)
                    {
                        Long.EncodeInto(values[i], payload.Slice(32 + (i * 32), 32), false);
                    }

                    break;
                }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Decodes an array of primitive numeric values.
        /// </summary>
        public static TInner[] Decode(ReadOnlyMemory<byte> bytes, int metaDataOffset)
        {
            var (data, length) = DecodeArrayPayload(bytes, metaDataOffset);

            switch(typeof(TInner))
            {
                case Type us8 when us8 == typeof(byte):
                {
                    byte[] arr = new byte[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = Byte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s8 when s8 == typeof(sbyte):
                {
                    sbyte[] arr = new sbyte[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = SByte.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us16 when us16 == typeof(ushort):
                {
                    ushort[] arr = new ushort[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = UShort.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s16 when s16 == typeof(short):
                {
                    short[] arr = new short[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = Short.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type us32 when us32 == typeof(uint):
                {
                    uint[] arr = new uint[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = UInt.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s32 when s32 == typeof(int):
                {
                    int[] arr = new int[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = Int.Decode(slot.Span);
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
                        arr[i] = ULong.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                case Type s64 when s64 == typeof(long):
                {
                    long[] arr = new long[length];
                    for(int i = 0; i < length; i++)
                    {
                        var slot = data[(i * 32)..((i * 32) + 32)];
                        arr[i] = Long.Decode(slot.Span);
                    }
                    return (TInner[]) (object) arr;
                }
                default:
                    throw new ArgumentException($"Expected primitive number type, got {typeof(TInner)}");

            }
        }
    }
}
