using EtherSharp.Types;

namespace EtherSharp.Crypto;
/// <summary>
/// Implementation of EIP55 address checksums.
/// <a href="https://eips.ethereum.org/EIPS/eip-55">EIP55 Spec</a>
/// </summary>
public static class EIP55
{
    private const string LOWER_HEX_ALPHABET = "0123456789abcdef";

    /// <summary>
    /// Formats the address to an EIP55 checksummed address string.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static string FormatAddress(in Address address)
    {
        var addressValue = address;
        Span<byte> asciiBytes = stackalloc byte[40];
        WriteLowercaseAsciiHex(addressValue.Span, asciiBytes);

        Span<byte> hash = stackalloc byte[32];
        Keccak256.TryHashData(asciiBytes, hash);

        return String.Create(
            42,
            hash,
            (span, hashBytes) =>
            {
                "0x".CopyTo(span);

                var bytes = addressValue.Span;
                for(int i = 0; i < bytes.Length; i++)
                {
                    byte value = bytes[i];
                    span[(i * 2) + 2] = ApplyChecksumCase(ToLowerHexChar(value >> 4), hashBytes[i], 4);
                    span[(i * 2) + 3] = ApplyChecksumCase(ToLowerHexChar(value & 0x0F), hashBytes[i], 0);
                }
            }
        );
    }

    private static void WriteLowercaseAsciiHex(ReadOnlySpan<byte> bytes, Span<byte> destination)
    {
        for(int i = 0; i < bytes.Length; i++)
        {
            byte value = bytes[i];
            destination[i * 2] = (byte) ToLowerHexChar(value >> 4);
            destination[(i * 2) + 1] = (byte) ToLowerHexChar(value & 0x0F);
        }
    }

    private static char ApplyChecksumCase(char c, byte hashByte, int shift)
        => c is >= 'a' and <= 'f' && ((hashByte >> shift) & 0x0F) >= 8
            ? Char.ToUpperInvariant(c)
            : c;

    private static char ToLowerHexChar(int value)
        => LOWER_HEX_ALPHABET[value];
}
