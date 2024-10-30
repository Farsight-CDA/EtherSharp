using EtherSharp.ABI.Fixed;

namespace EtherSharp.ABI.Types;

internal abstract partial class FixedEncodeType<T>(T value) : IFixedEncodeType
{
    public uint MetadataSize => 32;

    public T Value { get; } = value;

    public abstract void Encode(Span<byte> buffer);

    public class Int8(sbyte value) : FixedEncodeType<sbyte>(value)
    {
        public override void Encode(Span<byte> values)
        {
            values[^1] = (byte) Value;

            if(Value < 0)
            {
                values[..(32 - 1)].Fill(byte.MaxValue);
            }
        }
    }

    public class UInt8(byte value) : FixedEncodeType<byte>(value)
    {

        public override void Encode(Span<byte> values) => values[^1] = Value;
    }

    public class Int16(short value) : FixedEncodeType<short>(value)
    {

        public override void Encode(Span<byte> buffer)
        {

            if(!BitConverter.TryWriteBytes(buffer[(32 - 2)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 2)..].Reverse();

            }
            if(Value < 0)
            {
                buffer[..(32 - 2)].Fill(byte.MaxValue);
            }
        }
    }

    public class UInt16(ushort value) : FixedEncodeType<ushort>(value)
    {

        public override void Encode(Span<byte> buffer)
        {

            if(!BitConverter.TryWriteBytes(buffer[(32 - 2)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                buffer[(32 - 2)..].Reverse();

            }
        }
    }
}
