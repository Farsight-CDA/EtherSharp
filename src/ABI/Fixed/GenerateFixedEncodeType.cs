﻿using EtherSharp.ABI.Fixed;
using System.Numerics;

namespace EtherSharp.ABI.Types;
internal abstract partial class FixedEncodeType<T> : IFixedEncodeType
{
    public class GenerateInts : FixedEncodeType<int>
    {
        public GenerateInts(int value, int length) : base(value)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }

            uint unsignedValue = (uint) (value << (32 - length)) >> (32 - length);
            if(unsignedValue >> length != 0 && length != 32)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit signed integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {
            if(!BitConverter.TryWriteBytes(values[(32 - 4)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 4)..].Reverse();
            }
            if(Value < 0)
            {
                values[..(32 - 4)].Fill(byte.MaxValue);
            }
        }
    }

    public class GenerateUInts : FixedEncodeType<uint>
    {
        public GenerateUInts(uint value, int length) : base(value)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(value >> length != 0 && length != 32)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {
            if(!BitConverter.TryWriteBytes(values[(32 - 4)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[..(32 - 4)].Reverse();
            }
        }
    }

    public class GenerateLong : FixedEncodeType<long>
    {
        public GenerateLong(long value, int length) : base(value)
        {
            if(length < 24 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }

            bool posLimit = value > 0 && value > Math.Pow(2, length);
            bool negLimit = value < 0 && value < -Math.Pow(2, length) - 1;
            if(posLimit | negLimit)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit signed integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {

            if(!BitConverter.TryWriteBytes(values[(32 - 8)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 8)..].Reverse();
            }
            if(Value < 0)
            {
                values[..(32 - 8)].Fill(byte.MaxValue);
            }
        }
    }
    public class GenerateULong : FixedEncodeType<ulong>
    {
        public GenerateULong(ulong value, int length) : base(value)
        {
            if(length < 32 || length > 64 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(value >> length != 0 && length != 64)
            {
                throw new ArgumentException($"Value is too large to fit in a {length}-bit unsigned integer", nameof(value));
            }
        }

        public override void Encode(Span<byte> values)
        {
            if(!BitConverter.TryWriteBytes(values[(32 - 8)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 8)..].Reverse();
            }
        }
    }

    public class GenerateBigInteger : FixedEncodeType<BigInteger>
    {
        private readonly int _length;
        private readonly bool _isUnsigned;
        private readonly int _byteCount;

        public GenerateBigInteger(BigInteger value, bool isUnsigned, int length) : base(value)
        {
            if(length < 64 || length > 256 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }
            if(isUnsigned && value.Sign == -1)
            {
                throw new ArgumentException("Value was negative for unsigned fixed type");
            }

            _length = length;
            _isUnsigned = isUnsigned;
            _byteCount = value.GetByteCount(isUnsigned);

            if(_byteCount > length / 8)
            {
                throw new ArgumentException($"Value is too large to fit in a {_length}-bit {(isUnsigned ? "un" : "")}signed integer", nameof(value));
            }
        }
        public override void Encode(Span<byte> values)
        {
            if(!Value.TryWriteBytes(values[(32 - _byteCount)..], out _, isBigEndian: true, isUnsigned: _isUnsigned))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(Value.Sign < 0)
            {
                values[..(32 - _byteCount)].Fill(byte.MaxValue);
            }
        }
    }
}