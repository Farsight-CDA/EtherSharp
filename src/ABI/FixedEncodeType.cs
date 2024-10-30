namespace EtherSharp.ABI;

internal interface IFixedEncodeType : IEncodeType;
public abstract partial class FixedEncodeType<T>(T value) : IFixedEncodeType
{
    public T Value { get; } = value;

    public int MetadataSize => 32;

    public int PayloadSize => 0;

    public abstract void Encode(Span<byte> values);
    public void Encode(Span<byte> metadata, Span<byte> payload, int payloadOffset) => throw new NotImplementedException();

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

        public override void Encode(Span<byte> values)
        {

            if(!BitConverter.TryWriteBytes(values[(32 - 2)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 2)..].Reverse();

            }
            if(Value < 0)
            {
                values[..(32 - 2)].Fill(byte.MaxValue);
            }
        }
    }

    public class UInt16(ushort value) : FixedEncodeType<ushort>(value)
    {

        public override void Encode(Span<byte> values)
        {

            if(!BitConverter.TryWriteBytes(values[(32 - 2)..], Value))
            {
                throw new InvalidOperationException("Could Not Wryte Bytes");
            }
            if(BitConverter.IsLittleEndian)
            {
                values[(32 - 2)..].Reverse();

            }
        }
    }
}
