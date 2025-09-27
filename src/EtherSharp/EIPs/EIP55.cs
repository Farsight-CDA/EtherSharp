using EtherSharp.Crypto;
using EtherSharp.Types;
using System.Text;

namespace EtherSharp.EIPs;
/// <summary>
/// Implementation of EIP55 address checksums.
/// <a href="https://eips.ethereum.org/EIPS/eip-55">EIP55 Spec</a>
/// </summary>
public static class EIP55
{
    /// <summary>
    /// Formats the address to an EIP55 checksummed address string.
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public static string FormatAddress(Address address)
    {
        Span<char> lowercaseHex = stackalloc char[40];
        Span<byte> asciiBytes = stackalloc byte[40];

        address.String.AsSpan(2).ToLowerInvariant(lowercaseHex);
        Encoding.ASCII.TryGetBytes(lowercaseHex, asciiBytes, out _);

        Span<byte> hash = stackalloc byte[32];
        Keccak256.TryHashData(asciiBytes, hash);

        return String.Create(
            42,
            hash,
            (span, hashBytes) =>
            {
                "0x".CopyTo(span);

                for(int i = 0; i < 40; i++)
                {
                    char c = address.String[i + 2];
                    byte hashByte = hashBytes[i / 2];
                    int shift = 4 * (1 - (i % 2));

                    span[i + 2] = Char.IsLetter(c) &&
                        ((hashByte >> shift) & 0x0F) >= 8
                        ? Char.ToUpperInvariant(c)
                        : Char.ToLowerInvariant(c);
                }
            }
        );
    }
}