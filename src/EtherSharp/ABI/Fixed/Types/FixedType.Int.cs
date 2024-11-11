﻿using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    internal class Int : FixedType<int>
    {
        public Int(int value, int length) : base(value)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(length != 32 && ((value > 0 && value >> (length - 1) != 0) || (value < 0 && value >> (length - 1) != -1)))
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit signed integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> buffer)
            => EncodeInto(Value, buffer);

        public static void EncodeInto(int value, Span<byte> buffer)
        {
            if(!BitConverter.TryWriteBytes(buffer[(32 - 4)..], value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 4)..].Reverse();
            }
            if(value < 0)
            {
                buffer[..(32 - 4)].Fill(byte.MaxValue);
            }
        }

        public static int Decode(ReadOnlySpan<byte> bytes)
        {
            int value = BitConverter.ToInt32(bytes[(32 - 4)..]);

            if(BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            return value;
        }
    }
}