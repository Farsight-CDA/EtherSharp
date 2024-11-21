using System.Numerics;

namespace EtherSharp.RLP;
public ref struct RLPEncoder
{
    public static int GetIntSize(uint value)
        => value < 128
            ? 1
            :  GetEncodedStringLength(GetSignificantByteCount(value));
    public static int GetIntSize(ulong value)
        => value < 128
            ? 1
            : GetEncodedStringLength(GetSignificantByteCount(value));
    public static int GetIntSize(BigInteger value)
    {
        if(value < 0)
        {
            throw new NotSupportedException();
        }
        //
        return value < 128
            ? 1
            : GetEncodedStringLength(value.GetByteCount(true));
    }

    public static int GetEncodedStringLength(int byteCount)
        => byteCount < 56
            ? byteCount + 1
            : byteCount + GetSignificantByteCount((uint) byteCount) + 1;

    public static int GetStringSize(ReadOnlySpan<byte> data)
        => data.Length == 1 && data[0] < 128
            ? 1
            : GetPrefixLength(data.Length) + data.Length;

    public static int GetListSize(int elementSize)
        => GetPrefixLength(elementSize) + elementSize;

    public static int GetPrefixLength(int byteCount)
        => byteCount < 56
            ? 1
            : 1 + GetSignificantByteCount((uint) byteCount);

    public static int GetSignificantByteCount(uint value)
    {
        int lengthBits = 32 - BitOperations.LeadingZeroCount(value);
        int lengthBytes = (lengthBits + 7) / 8;
        return lengthBytes;
    }
    public static int GetSignificantByteCount(ulong value)
    {
        int lengthBits = 64 - BitOperations.LeadingZeroCount(value);
        int lengthBytes = (lengthBits + 7) / 8;
        return lengthBytes;
    }

    private Span<byte> _destination;

    public RLPEncoder(Span<byte> destination)
    {
        _destination = destination;
    }

    public RLPEncoder EncodeInt(uint value)
    {
        if (value == 0)
        {
            return EncodeString();
        }
        else if (value < 128)
        {
            return EncodeString((byte) value);
        }
        else
        {
            int significantBytes = GetSignificantByteCount(value);
            Span<byte> buffer = stackalloc byte[4];
            BitConverter.TryWriteBytes(buffer, value);
            if(BitConverter.IsLittleEndian)
            {
                buffer.Reverse();
            }

            _destination[0] = (byte) (0x80 + significantBytes);
            buffer[^significantBytes..].CopyTo(_destination[1..]);
            _destination = _destination[(significantBytes + 1)..];
        }

        return this;
    }

    public RLPEncoder EncodeInt(ulong value)
    {
        if(value == 0)
        {
            return EncodeString();
        }
        else if(value < 128)
        {
            return EncodeString((byte) value);
        }
        else
        {
            int significantBytes = GetSignificantByteCount(value);
            Span<byte> buffer = stackalloc byte[8];
            BitConverter.TryWriteBytes(buffer, value);
            if(BitConverter.IsLittleEndian)
            {
                buffer.Reverse();
            }

            _destination[0] = (byte) (0x80 + significantBytes);
            buffer[^significantBytes..].CopyTo(_destination[1..]);
            _destination = _destination[(significantBytes + 1)..];
        }

        return this;
    }

    public RLPEncoder EncodeInt(BigInteger value)
    {
        if (value < 0)
        {
            throw new NotSupportedException();
        }

        if(value == 0)
        {
            return EncodeString();
        }
        else if(value < 128)
        {
            return EncodeString((byte) value);
        }
        else
        {
            int significantBytes = value.GetByteCount(true);

            _destination[0] = (byte) (0x80 + significantBytes);

            value.TryWriteBytes(_destination[1..], out _, true, true);

            Span<byte> buffer = stackalloc byte[significantBytes];

            _destination = _destination[(significantBytes + 1)..];
        }

        return this;
    }

    public RLPEncoder EncodeString(params ReadOnlySpan<byte> data)
    {
        if (data.Length == 1 && data[0] < 128)
        {
            _destination[0] = data[0];
            _destination = _destination[1..];
        }
        else if(data.Length < 56)
        {
            _destination[0] = (byte) (0x80 + data.Length);
            data.CopyTo(_destination[1..]);
            _destination = _destination[(data.Length + 1)..];
        }
        else
        {
            int significantLengthBytes = GetSignificantByteCount((uint) data.Length);

            _destination[0] = (byte) (0xb7 + significantLengthBytes);

            BitConverter.TryWriteBytes(_destination[1..5], (uint) data.Length);
            if(BitConverter.IsLittleEndian && significantLengthBytes > 1)
            {
                _destination[1..(1 + significantLengthBytes)].Reverse();
            }

            data.CopyTo(_destination[(significantLengthBytes + 1)..]);
            _destination = _destination[(significantLengthBytes + 1 + data.Length)..];
        }

        return this;
    }

    public RLPEncoder EncodeList(int listLength)
    {
        if (listLength < 56)
        {
            _destination[0] = (byte) (0xc0 + listLength);
            _destination = _destination[1..];
        }
        else
        {
            int significantLengthBytes = GetSignificantByteCount((uint) listLength);
            Span<byte> lengthBuffer = stackalloc byte[4];
            BitConverter.TryWriteBytes(lengthBuffer, (uint) listLength);
            if (BitConverter.IsLittleEndian)
            {
                lengthBuffer.Reverse();
            }

            _destination[0] = (byte) (0xf7 + significantLengthBytes);
            lengthBuffer[^significantLengthBytes..].CopyTo(_destination[1..]);
            _destination = _destination[(1 + significantLengthBytes)..];
        }

        return this;
    }
}