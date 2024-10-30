using System.Buffers.Binary;

namespace EtherSharp.ABI.Fixed;
internal abstract partial class FixedType<T>
{
    public class Int : FixedType<int>
    {
        public Int(int value, int length) : base(value)
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

        public static int Decode(Memory<byte> bytes, int length)
        {
            if(length < 24 || length > 32 || length % 8 != 0)
            {
                throw new ArgumentException("Invalid bit size for fixed type", nameof(length));
            }

            if(!int.TryParse(bytes.Span[..4], out int value))
            {
                throw new InvalidDataException($"Could Not Read Bytes of type Int{length}");
            }
            if(BitConverter.IsLittleEndian)
            {
                value = BinaryPrimitives.ReverseEndianness(value);
            }
            return value;
        }
    }
}
