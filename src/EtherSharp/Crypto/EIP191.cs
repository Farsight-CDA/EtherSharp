using EtherSharp.Types;
using System.Buffers;
using System.Buffers.Text;
using System.Text;

namespace EtherSharp.Crypto;

/// <summary>
/// Provides EIP-191 hashing for personal messages (version 0x45) and intended validators (version 0x00).
/// EIP-712 structured data (version 0x01) is handled by <see cref="EIP712"/>.
/// </summary>
public static class EIP191
{
    private const int MAX_STACKALLOC_BYTES = 1024;
    private static ReadOnlySpan<byte> PersonalPrefix => "\u0019Ethereum Signed Message:\n"u8;

    /// <summary>
    /// Hashes a personal message as keccak256("\x19Ethereum Signed Message:\n" + decimal byte length + message).
    /// </summary>
    /// <param name="message">The raw message bytes.</param>
    /// <returns>The digest to sign.</returns>
    public static Bytes32 HashPersonalMessage(ReadOnlySpan<byte> message)
    {
        Span<byte> prefix = stackalloc byte[PersonalPrefix.Length + 10];
        int prefixLength = WritePersonalPrefix(prefix, message.Length);
        return HashPrefixed(prefix[..prefixLength], message);
    }

    /// <summary>
    /// Hashes the UTF-8 bytes of a personal message using the EIP-191 version 0x45 envelope.
    /// </summary>
    /// <param name="message">The text to encode as UTF-8.</param>
    /// <returns>The digest to sign.</returns>
    public static Bytes32 HashPersonalMessage(string message)
    {
        ArgumentNullException.ThrowIfNull(message);

        int byteCount = Encoding.UTF8.GetByteCount(message);
        Span<byte> prefix = stackalloc byte[PersonalPrefix.Length + 10];
        int prefixLength = WritePersonalPrefix(prefix, byteCount);
        int length = checked(prefixLength + byteCount);
        byte[]? rented = null;
        var payload = length <= MAX_STACKALLOC_BYTES
            ? stackalloc byte[length]
            : (rented = ArrayPool<byte>.Shared.Rent(length));

        try
        {
            prefix[..prefixLength].CopyTo(payload);
            _ = Encoding.UTF8.GetBytes(message.AsSpan(), payload[prefixLength..]);
            return Keccak256.HashData(payload[..length]);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    /// <summary>
    /// Hashes data intended for a validator as keccak256(0x19 || 0x00 || validator || data).
    /// </summary>
    /// <param name="validator">The 20-byte intended validator address.</param>
    /// <param name="data">The raw data to sign.</param>
    /// <returns>The digest to sign.</returns>
    public static Bytes32 HashIntendedValidator(in Address validator, ReadOnlySpan<byte> data)
    {
        Span<byte> prefix = stackalloc byte[2 + Address.BYTES_LENGTH];
        prefix[0] = 0x19;
        prefix[1] = 0x00;
        validator.CopyTo(prefix[2..]);
        return HashPrefixed(prefix, data);
    }

    private static int WritePersonalPrefix(Span<byte> prefix, int byteCount)
    {
        PersonalPrefix.CopyTo(prefix);
        _ = Utf8Formatter.TryFormat(byteCount, prefix[PersonalPrefix.Length..], out int digitsWritten);
        return PersonalPrefix.Length + digitsWritten;
    }

    private static Bytes32 HashPrefixed(ReadOnlySpan<byte> prefix, ReadOnlySpan<byte> data)
    {
        int length = checked(prefix.Length + data.Length);
        byte[]? rented = null;
        var payload = length <= MAX_STACKALLOC_BYTES
            ? stackalloc byte[length]
            : (rented = ArrayPool<byte>.Shared.Rent(length));

        try
        {
            prefix.CopyTo(payload);
            data.CopyTo(payload[prefix.Length..]);
            return Keccak256.HashData(payload[..length]);
        }
        finally
        {
            if(rented is not null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}
