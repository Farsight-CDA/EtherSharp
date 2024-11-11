﻿using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Long : FixedType<long>
    {
        public Long(long value, int length) : base(value)
        {
            if(length < 24 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(length != 64 && ((value > 0 && value >> (length - 1) != 0) || (value < 0 && value >> (length - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit signed integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(long value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 8)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 8)..].Reverse();
            }
            if(value < 0)
            {
                buffer[..(32 - 8)].Fill(byte.MaxValue);
            }
        }

        public static long Decode(ReadOnlySpan<byte> bytes)
        {
            long value = BitConverter.ToInt64(bytes[(32 - 8)..]);

            if(BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            return value;
        }
    }
}