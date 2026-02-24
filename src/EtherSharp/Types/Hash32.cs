using EtherSharp.Common;
using EtherSharp.Common.Converter;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;

/// <summary>
/// Represents a 32-byte hash value encoded as a 0x-prefixed 64-nybble hex string.
/// </summary>
[JsonConverter(typeof(Hash32Converter))]
public struct Hash32 : IEquatable<Hash32>, IComparable<Hash32>
{
    /// <summary>
    /// Byte length of a hash.
    /// </summary>
    public const int BYTE_LENGTH = 32;

    /// <summary>
    /// Character length of a 0x-prefixed hash string.
    /// </summary>
    public const int STRING_LENGTH = 66;

    [InlineArray(BYTE_LENGTH)]
    private struct ByteStorage32
    {
        private byte _element0;
    }

    private ByteStorage32 _bytes;

    /// <summary>
    /// Returns the zero hash.
    /// </summary>
    public static Hash32 Zero => default;

    /// <summary>
    /// Returns the raw 32-byte span.
    /// </summary>
    public ReadOnlySpan<byte> Bytes
        => MemoryMarshal.CreateReadOnlySpan(ref _bytes[0], BYTE_LENGTH);

    /// <summary>
    /// Parses a 32-byte hash from hex.
    /// </summary>
    public static Hash32 Parse(string value)
        => Parse(value.AsSpan());

    /// <summary>
    /// Parses a 32-byte hash from hex.
    /// </summary>
    public static Hash32 Parse(ReadOnlySpan<char> value)
        => !TryParse(value, out var parsed)
            ? throw new FormatException("Expected a 32-byte hash with optional 0x prefix.")
            : parsed;

    /// <summary>
    /// Tries to parse a 32-byte hash from hex.
    /// </summary>
    public static bool TryParse(string? value, out Hash32 parsed)
    {
        if(value is null)
        {
            parsed = default;
            return false;
        }

        return TryParse(value.AsSpan(), out parsed);
    }

    /// <summary>
    /// Tries to parse a 32-byte hash from hex.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<char> value, out Hash32 parsed)
    {
        parsed = default;
        var span = value;

        if(span.Length == STRING_LENGTH)
        {
            if(span[0] != '0' || (span[1] != 'x' && span[1] != 'X'))
            {
                return false;
            }

            span = span[2..];
        }
        else if(span.Length != BYTE_LENGTH * 2)
        {
            return false;
        }

        Span<byte> bytes = stackalloc byte[BYTE_LENGTH];
        var status = Convert.FromHexString(span, bytes, out int charsConsumed, out int bytesWritten);
        if(status != OperationStatus.Done
            || charsConsumed != BYTE_LENGTH * 2
            || bytesWritten != BYTE_LENGTH)
        {
            return false;
        }

        bytes.CopyTo(MemoryMarshal.CreateSpan(ref parsed._bytes[0], BYTE_LENGTH));
        return true;
    }

    /// <summary>
    /// Creates a hash from exactly 32 bytes.
    /// </summary>
    public static Hash32 FromBytes(ReadOnlySpan<byte> bytes)
    {
        if(bytes.Length != BYTE_LENGTH)
        {
            throw new ArgumentException("Hash32 requires exactly 32 bytes", nameof(bytes));
        }

        var hash = default(Hash32);
        bytes.CopyTo(MemoryMarshal.CreateSpan(ref hash._bytes[0], BYTE_LENGTH));
        return hash;
    }

    /// <summary>
    /// Copies the hash bytes to a new array.
    /// </summary>
    public byte[] ToArray()
    {
        byte[] copy = new byte[BYTE_LENGTH];
        Bytes.CopyTo(copy);
        return copy;
    }

    /// <inheritdoc/>
    public override string ToString()
        => HexUtils.ToPrefixedHexString(Bytes);

    /// <inheritdoc/>
    [OverloadResolutionPriority(1)]
    public bool Equals(in Hash32 other)
        => ((LoadU64Raw(0) ^ other.LoadU64Raw(0))
            | (LoadU64Raw(8) ^ other.LoadU64Raw(8))
            | (LoadU64Raw(16) ^ other.LoadU64Raw(16))
            | (LoadU64Raw(24) ^ other.LoadU64Raw(24))) == 0;

    /// <inheritdoc/>
    public bool Equals(Hash32 other)
        => Equals(in other);

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Hash32 other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
        => HashCode.Combine(LoadU64BE(0), LoadU64BE(8), LoadU64BE(16), LoadU64BE(24));

    /// <inheritdoc/>
    [OverloadResolutionPriority(1)]
    public int CompareTo(in Hash32 other)
    {
        int cmp = LoadU64BE(0).CompareTo(other.LoadU64BE(0));
        if(cmp != 0)
        {
            return cmp;
        }

        cmp = LoadU64BE(8).CompareTo(other.LoadU64BE(8));
        if(cmp != 0)
        {
            return cmp;
        }

        cmp = LoadU64BE(16).CompareTo(other.LoadU64BE(16));
        return cmp != 0 ? cmp : LoadU64BE(24).CompareTo(other.LoadU64BE(24));
    }

    /// <inheritdoc/>
    public int CompareTo(Hash32 other)
        => CompareTo(in other);

    /// <inheritdoc/>
    public static bool operator ==(in Hash32 left, in Hash32 right)
        => left.Equals(in right);

    /// <inheritdoc/>
    public static bool operator !=(in Hash32 left, in Hash32 right)
        => !left.Equals(in right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong LoadU64Raw(int offset)
        => Unsafe.ReadUnaligned<ulong>(ref Unsafe.Add(ref _bytes[0], offset));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ulong LoadU64BE(int offset)
    {
        ulong value = LoadU64Raw(offset);
        return BitConverter.IsLittleEndian ? BinaryPrimitives.ReverseEndianness(value) : value;
    }
}
