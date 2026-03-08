using EtherSharp.Common;
using EtherSharp.Common.Converter;
using EtherSharp.Crypto;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;
/// <summary>
/// Represents an Ethereum compatible address.
/// </summary>
[JsonConverter(typeof(AddressConverter))]
public readonly struct Address : IEquatable<Address>, IComparable<Address>
{
    /// <summary>
    /// Character length of an address including 0x prefix.
    /// </summary>
    public const int STRING_LENGTH = 42;
    /// <summary>
    /// Byte length of an address.
    /// </summary>
    public const int BYTES_LENGTH = 20;

    /// <summary>
    /// Returns the Zero address.
    /// </summary>
    public static Address Zero => default;

    internal readonly Bytes20 _bytes;

    private Address(in Bytes20 bytes)
    {
        _bytes = bytes;
    }

    /// <summary>
    /// Creates an <see cref="Address"/> instance given its string representation.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Address FromString(string s)
        => new(Bytes20.Parse(s));

    /// <summary>
    /// Creates an <see cref="Address"/> instance given its string representation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Address Parse(string value)
        => FromString(value);

    /// <summary>
    /// Attempts to parse an address string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parsed"></param>
    /// <returns></returns>
    public static bool TryParse(string? value, out Address parsed)
    {
        if(Bytes20.TryParse(value, out var bytes))
        {
            parsed = new Address(bytes);
            return true;
        }

        parsed = default;
        return false;
    }

    /// <summary>
    /// Attempts to parse an address string.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="parsed"></param>
    /// <returns></returns>
    public static bool TryParse(ReadOnlySpan<char> value, out Address parsed)
    {
        if(Bytes20.TryParse(value, out var bytes))
        {
            parsed = new Address(bytes);
            return true;
        }

        parsed = default;
        return false;
    }

    /// <summary>
    /// Calculates the EIP55 formatted string representation of this address.
    /// </summary>
    /// <returns></returns>
    public readonly string ToEIP55String()
        => EIP55.FormatAddress(this);

    /// <inheritdoc/>
    public readonly override string ToString()
        => HexUtils.ToPrefixedHexString(_bytes.DangerousGetReadOnlySpan());

    /// <summary>
    /// Copies the address bytes into the destination span.
    /// </summary>
    public readonly void CopyTo(Span<byte> destination)
        => _bytes.CopyTo(destination);

    /// <summary>
    /// Tries to copy the address bytes into the destination span.
    /// </summary>
    public readonly bool TryWriteTo(Span<byte> destination)
        => _bytes.TryWriteTo(destination);

    /// <summary>
    /// Copies the address bytes into a new array.
    /// </summary>
    public readonly byte[] ToArray()
        => _bytes.ToArray();

    /// <summary>
    /// Creates an <see cref="Address"/> instance given its binary representation.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Address FromBytes(ReadOnlySpan<byte> b)
        => new(Bytes20.FromBytes(b));

    /// <inheritdoc/>
    [OverloadResolutionPriority(1)]
    public readonly bool Equals(in Address other)
        => _bytes.Equals(other._bytes);

    /// <inheritdoc/>
    public readonly bool Equals(Address other)
        => Equals(in other);

    /// <inheritdoc/>
    public readonly override bool Equals(object? obj)
        => obj is Address other && Equals(other);

    /// <inheritdoc/>
    public readonly override int GetHashCode()
        => _bytes.GetHashCode();

    /// <inheritdoc/>
    [OverloadResolutionPriority(1)]
    public readonly int CompareTo(in Address other)
        => _bytes.CompareTo(other._bytes);

    /// <inheritdoc/>
    public readonly int CompareTo(Address other)
        => CompareTo(in other);

    /// <inheritdoc/>
    public static bool operator ==(in Address a, in Address b)
        => a.Equals(in b);

    /// <inheritdoc/>
    public static bool operator ==(in Address a, string? b)
        => b is not null && TryParse(b, out var parsed) && a == parsed;

    /// <inheritdoc/>
    public static bool operator ==(string? a, in Address b)
        => b == a;

    /// <inheritdoc/>
    public static bool operator !=(in Address a, in Address b)
        => !a.Equals(in b);

    /// <inheritdoc/>
    public static bool operator !=(in Address a, string? b)
        => !(a == b);

    /// <inheritdoc/>
    public static bool operator !=(string? a, in Address b)
        => !(a == b);

    /// <inheritdoc/>
    public static bool operator <(in Address a, in Address b)
        => a.CompareTo(in b) < 0;

    /// <inheritdoc/>
    public static bool operator <=(in Address a, in Address b)
        => a.CompareTo(in b) <= 0;

    /// <inheritdoc/>
    public static bool operator >(in Address a, in Address b)
        => a.CompareTo(in b) > 0;

    /// <inheritdoc/>
    public static bool operator >=(in Address a, in Address b)
        => a.CompareTo(in b) >= 0;

    /// <inheritdoc/>
    public static implicit operator Address(string a)
        => FromString(a);
}
