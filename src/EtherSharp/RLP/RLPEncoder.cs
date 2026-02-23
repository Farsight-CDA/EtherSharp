using EtherSharp.Numerics;
using System.Buffers.Binary;
using System.Numerics;

namespace EtherSharp.RLP;

/// <summary>
/// Writes Recursive Length Prefix (RLP) values into a preallocated destination buffer.
/// </summary>
/// <remarks>
/// The encoder is append-only: each call writes at the current cursor and advances it,
/// which enables fluent chained encoding without additional allocations.
/// </remarks>
public ref struct RLPEncoder
{
    /// <summary>
    /// Gets the encoded RLP length for a <see cref="UInt32"/> value.
    /// </summary>
    public static int GetIntSize(uint value)
        => value < 128
            ? 1
            : GetEncodedStringLength(GetSignificantByteCount(value));

    /// <summary>
    /// Gets the encoded RLP length for a <see cref="UInt64"/> value.
    /// </summary>
    public static int GetIntSize(ulong value)
        => value < 128
            ? 1
            : GetEncodedStringLength(GetSignificantByteCount(value));

    /// <summary>
    /// Gets the encoded RLP length for a <see cref="UInt256"/> value.
    /// </summary>
    public static int GetIntSize(UInt256 value)
        => value < 128
            ? 1
            : GetEncodedStringLength(GetSignificantByteCount(value));

    /// <summary>
    /// Gets the encoded RLP length for a byte string payload of the specified size.
    /// </summary>
    /// <param name="byteCount">The number of payload bytes before RLP prefixing.</param>
    public static int GetEncodedStringLength(int byteCount)
        => byteCount < 56
            ? byteCount + 1
            : byteCount + GetSignificantByteCount((uint) byteCount) + 1;

    /// <summary>
    /// Gets the encoded RLP length for the provided byte span when encoded as a string element.
    /// </summary>
    public static int GetStringSize(ReadOnlySpan<byte> data)
        => data.Length == 1 && data[0] < 128
            ? 1
            : GetPrefixLength(data.Length) + data.Length;

    /// <summary>
    /// Gets the encoded RLP length for a list with the specified encoded content length.
    /// </summary>
    /// <param name="elementSize">The total encoded byte size of all list elements.</param>
    public static int GetListSize(int elementSize)
        => GetPrefixLength(elementSize) + elementSize;

    /// <summary>
    /// Gets the RLP prefix length for a payload of the specified byte size.
    /// </summary>
    public static int GetPrefixLength(int byteCount)
        => byteCount < 56
            ? 1
            : 1 + GetSignificantByteCount((uint) byteCount);

    /// <summary>
    /// Gets the number of non-leading-zero bytes required to represent a <see cref="UInt32"/>.
    /// </summary>
    public static int GetSignificantByteCount(uint value)
    {
        int lengthBits = 32 - BitOperations.LeadingZeroCount(value);
        int lengthBytes = (lengthBits + 7) / 8;
        return lengthBytes;
    }

    /// <summary>
    /// Gets the number of non-leading-zero bytes required to represent a <see cref="UInt64"/>.
    /// </summary>
    public static int GetSignificantByteCount(ulong value)
    {
        int lengthBits = 64 - BitOperations.LeadingZeroCount(value);
        int lengthBytes = (lengthBits + 7) / 8;
        return lengthBytes;
    }

    /// <summary>
    /// Gets the number of non-leading-zero bytes required to represent a <see cref="UInt256"/>.
    /// </summary>
    public static int GetSignificantByteCount(UInt256 value)
    {
        int lengthBits = 256 - UInt256.LeadingZeroCount(value);
        int lengthBytes = (lengthBits + 7) / 8;
        return lengthBytes;
    }

    private Span<byte> _destination;

    /// <summary>
    /// Creates a new encoder that writes into the provided destination span.
    /// </summary>
    /// <param name="destination">The preallocated buffer that receives encoded bytes.</param>
    public RLPEncoder(Span<byte> destination)
    {
        _destination = destination;
    }

    /// <summary>
    /// Encodes a <see cref="UInt32"/> as an RLP string element.
    /// </summary>
    public RLPEncoder EncodeInt(uint value)
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
            _destination[0] = (byte) (0x80 + significantBytes);

            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);
            buffer[^significantBytes..].CopyTo(_destination[1..]);

            _destination = _destination[(significantBytes + 1)..];
        }

        return this;
    }

    /// <summary>
    /// Encodes a <see cref="UInt64"/> as an RLP string element.
    /// </summary>
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

            _destination[0] = (byte) (0x80 + significantBytes);

            Span<byte> buffer = stackalloc byte[8];
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);
            buffer[^significantBytes..].CopyTo(_destination[1..]);

            _destination = _destination[(significantBytes + 1)..];
        }

        return this;
    }

    /// <summary>
    /// Encodes a <see cref="UInt256"/> as an RLP string element.
    /// </summary>
    public RLPEncoder EncodeInt(UInt256 value)
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
            Span<byte> buffer = stackalloc byte[32];
            BinaryPrimitives.WriteUInt256BigEndian(buffer, value);

            buffer = buffer.TrimStart((byte) 0);

            _destination[0] = (byte) (0x80 + buffer.Length);
            buffer.CopyTo(_destination[1..]);
            _destination = _destination[(1 + buffer.Length)..];
        }

        return this;
    }

    /// <summary>
    /// Encodes a byte span as an RLP string element.
    /// </summary>
    /// <param name="data">The byte payload to encode.</param>
    public RLPEncoder EncodeString(params ReadOnlySpan<byte> data)
    {
        if(data.Length == 1 && data[0] < 128)
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

            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, (uint) data.Length);
            buffer[^significantLengthBytes..].CopyTo(_destination[1..]);

            data.CopyTo(_destination[(significantLengthBytes + 1)..]);
            _destination = _destination[(significantLengthBytes + 1 + data.Length)..];
        }

        return this;
    }

    /// <summary>
    /// Encodes an RLP list prefix for a list with the specified encoded payload length.
    /// </summary>
    /// <param name="listLength">The total encoded byte size of the list payload.</param>
    public RLPEncoder EncodeList(int listLength)
    {
        if(listLength < 56)
        {
            _destination[0] = (byte) (0xc0 + listLength);
            _destination = _destination[1..];
        }
        else
        {
            int significantLengthBytes = GetSignificantByteCount((uint) listLength);

            _destination[0] = (byte) (0xf7 + significantLengthBytes);

            Span<byte> buffer = stackalloc byte[4];
            BinaryPrimitives.WriteUInt32BigEndian(buffer, (uint) listLength);

            buffer[^significantLengthBytes..].CopyTo(_destination[1..]);

            _destination = _destination[(1 + significantLengthBytes)..];
        }

        return this;
    }
}
