using EtherSharp.Numerics;
using EtherSharp.Types;
using System.Buffers.Binary;

namespace EtherSharp.RLP;

/// <summary>
/// Reads Recursive Length Prefix (RLP) values from a source buffer.
/// </summary>
/// <remarks>
/// Only canonical encodings are accepted. Failed reads leave the cursor unchanged,
/// and returned spans are views into the source buffer.
/// </remarks>
public ref partial struct RLPDecoder
{
    private ReadOnlySpan<byte> _remaining;

    /// <summary>
    /// Gets whether all items have been consumed.
    /// </summary>
    public readonly bool IsComplete
        => _remaining.IsEmpty;

    /// <summary>
    /// Creates a new decoder that reads from the provided source span.
    /// </summary>
    /// <param name="source">The encoded RLP items to read.</param>
    public RLPDecoder(ReadOnlySpan<byte> source)
    {
        _remaining = source;
    }

    /// <summary>
    /// Reads the next item including its RLP prefix.
    /// </summary>
    /// <param name="item">The full encoded item. Its payload starts at <see cref="RLPHeader.PrefixLength"/>.</param>
    /// <param name="header">The header of the item.</param>
    /// <remarks>
    /// Only the header of the item is validated. Use <see cref="IsCanonical"/> to validate nested items.
    /// </remarks>
    public bool TryReadRaw(out ReadOnlySpan<byte> item, out RLPHeader header)
    {
        if(!RLPHeader.TryRead(_remaining, out header))
        {
            item = default;
            return false;
        }

        item = _remaining[..header.TotalLength];
        _remaining = _remaining[header.TotalLength..];
        return true;
    }

    /// <summary>
    /// Reads the next item as a string element.
    /// </summary>
    /// <param name="value">The string payload without its RLP prefix.</param>
    public bool TryReadString(out ReadOnlySpan<byte> value)
    {
        if(!RLPHeader.TryRead(_remaining, out var header) || header.IsList)
        {
            value = default;
            return false;
        }

        value = _remaining.Slice(header.PrefixLength, header.PayloadLength);
        _remaining = _remaining[header.TotalLength..];
        return true;
    }

    /// <summary>
    /// Reads the next item as a string element with exactly <paramref name="length"/> bytes.
    /// </summary>
    /// <param name="length">The required payload length.</param>
    /// <param name="value">The string payload without its RLP prefix.</param>
    public bool TryReadString(int length, out ReadOnlySpan<byte> value)
    {
        var candidate = this;
        if(!candidate.TryReadString(out value) || value.Length != length)
        {
            value = default;
            return false;
        }

        this = candidate;
        return true;
    }

    /// <summary>
    /// Reads the next item as a 20-byte <see cref="Address"/> string element.
    /// </summary>
    public bool TryReadAddress(out Address address)
    {
        if(!TryReadString(Address.BYTES_LENGTH, out var bytes))
        {
            address = default;
            return false;
        }

        address = Address.FromBytes(bytes);
        return true;
    }

    /// <summary>
    /// Reads the next item as a 32-byte <see cref="Bytes32"/> string element.
    /// </summary>
    public bool TryReadBytes32(out Bytes32 value)
    {
        if(!TryReadString(Bytes32.BYTE_LENGTH, out var bytes))
        {
            value = default;
            return false;
        }

        value = Bytes32.FromBytes(bytes);
        return true;
    }

    /// <summary>
    /// Reads the next item as a canonical <see cref="UInt32"/>.
    /// </summary>
    public bool TryReadInt(out uint value)
    {
        value = 0;
        var candidate = this;
        if(!candidate.TryReadString(out var bytes) || bytes.Length > sizeof(uint) || (!bytes.IsEmpty && bytes[0] == 0))
        {
            return false;
        }

        foreach(byte b in bytes)
        {
            value = (value << 8) | b;
        }

        this = candidate;
        return true;
    }

    /// <summary>
    /// Reads the next item as a canonical <see cref="UInt64"/>.
    /// </summary>
    public bool TryReadInt(out ulong value)
    {
        value = 0;
        var candidate = this;
        if(!candidate.TryReadString(out var bytes) || bytes.Length > sizeof(ulong) || (!bytes.IsEmpty && bytes[0] == 0))
        {
            return false;
        }

        foreach(byte b in bytes)
        {
            value = (value << 8) | b;
        }

        this = candidate;
        return true;
    }

    /// <summary>
    /// Reads the next item as a canonical <see cref="UInt256"/>.
    /// </summary>
    public bool TryReadInt(out UInt256 value)
    {
        var candidate = this;
        if(!candidate.TryReadString(out var bytes) || bytes.Length > 32 || (!bytes.IsEmpty && bytes[0] == 0))
        {
            value = default;
            return false;
        }

        Span<byte> buffer = stackalloc byte[32];
        buffer[..^bytes.Length].Clear();
        bytes.CopyTo(buffer[^bytes.Length..]);

        value = BinaryPrimitives.ReadUInt256BigEndian(buffer);
        this = candidate;
        return true;
    }

    /// <summary>
    /// Reads the next item as a list.
    /// </summary>
    /// <param name="list">A decoder over the list payload.</param>
    public bool TryReadList(out RLPDecoder list)
    {
        if(!RLPHeader.TryRead(_remaining, out var header) || !header.IsList)
        {
            list = default;
            return false;
        }

        list = new RLPDecoder(_remaining.Slice(header.PrefixLength, header.PayloadLength));
        _remaining = _remaining[header.TotalLength..];
        return true;
    }
}
