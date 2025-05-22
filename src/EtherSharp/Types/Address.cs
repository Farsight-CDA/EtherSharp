using System;
using System.Globalization;

namespace EtherSharp.Types;
/// <summary>
/// Represents an Ethereum compatible address.
/// </summary>
public class Address
{
    /// <summary>
    /// Returns the Zero address.
    /// </summary>
    public static Address Zero { get; } = new Address("0x0000000000000000000000000000000000000000", new byte[20]);

    private readonly byte[] _addressBytes;

    /// <summary>
    /// Returns the string representation of the address.
    /// </summary>
    public string String { get; }

    /// <summary>
    /// Returns the binary representation of the address.
    /// </summary>
    public ReadOnlySpan<byte> Bytes => _addressBytes;

    private Address(string s, byte[] b)
    {
        String = s;
        _addressBytes = b;
    }

    /// <summary>
    /// Creates an <see cref="Address"/> instance given its string representation.
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    public static Address FromString(string s)
    {
        if(s.StartsWith("0x", false, CultureInfo.InvariantCulture))
        {
            //
            return new Address(s, Convert.FromHexString(s.AsSpan()[2..]));
        }
        else
        {
            return new Address($"0x{s}", Convert.FromHexString(s));
        }
    }

    /// <summary>
    /// Creates an <see cref="Address"/> instance given its binary representation.
    /// </summary>
    /// <param name="b"></param>
    /// <returns></returns>
    public static Address FromBytes(ReadOnlySpan<byte> b)
        => new Address($"0x{Convert.ToHexString(b)}", b.ToArray());

    /// <inheritdoc/>
    public override bool Equals(object? obj)
        => obj is Address other && other._addressBytes.SequenceEqual(_addressBytes);

    /// <inheritdoc/>
    public override int GetHashCode()
        => String.GetHashCode();

    /// <inheritdoc/>
    public static bool operator ==(Address a, Address b)
        => Equals(a, b);

    /// <inheritdoc/>
    public static bool operator ==(Address a, string b)
        => b.StartsWith("0x")
            ? string.Equals(a.String, b, StringComparison.OrdinalIgnoreCase)
            : a.String.AsSpan(2).Equals(b, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public static bool operator ==(string a, Address b)
        => b == a;

    /// <inheritdoc/>
    public static bool operator !=(Address a, Address b)
        => !Equals(a, b);

    /// <inheritdoc/>
    public static bool operator !=(Address a, string b)
        => b.StartsWith("0x")
            ? !string.Equals(a.String, b, StringComparison.OrdinalIgnoreCase)
            : !a.String.AsSpan(2).Equals(b, StringComparison.OrdinalIgnoreCase);

    /// <inheritdoc/>
    public static bool operator !=(string a, Address b)
        => b != a;
}
