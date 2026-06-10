using EtherSharp.ABI;
using EtherSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Contract.Sections;

public static partial class SolidityErrors
{
    /// <summary>
    /// Built-in Solidity <c>Panic(uint256)</c> revert error.
    /// </summary>
    /// <param name="Type">Decoded panic type.</param>
    public sealed record Panic(PanicType Type) : ISolidityError<Panic>
    {
        /// <summary>
        /// Error signature used to calculate the signature bytes.
        /// </summary>
        public static string ErrorSignature { get; } = "Panic(uint256)";

        /// <summary>
        /// Hex encoded error signature bytes based on function signature: Panic(uint256)
        /// </summary>
        public static string SignatureHex { get; } = "0x4e487b71";

        /// <summary>
        /// Parsed bytes4 error selector based on signature: Panic(uint256)
        /// </summary>
        public static Bytes4 Signature { get; } = Bytes4.Parse(SignatureHex);

        /// <summary>
        /// Decodes Solidity <c>Panic(uint256)</c> revert data.
        /// </summary>
        /// <param name="data">Error data including selector and ABI-encoded arguments.</param>
        /// <returns>The decoded panic.</returns>
        public static Panic Decode(ReadOnlyMemory<byte> data)
        {
            var decoder = new AbiDecoder(data[4..]);
            return new Panic((PanicType) (byte) decoder.UInt256());
        }

        /// <summary>
        /// Checks whether the error data starts with the Solidity <c>Panic(uint256)</c> selector.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <returns><see langword="true" /> when the selector matches; otherwise <see langword="false" />.</returns>
        public static bool IsMatchingSignature(ReadOnlySpan<byte> errorData)
            => errorData.Length >= 4 && Signature == Bytes4.FromBytes(errorData[0..4]);

        /// <summary>
        /// Attempts to decode Solidity <c>Panic(uint256)</c> revert data.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <param name="parsedError">The decoded panic when successful.</param>
        /// <returns><see langword="true" /> when the selector matches and the panic was decoded; otherwise <see langword="false" />.</returns>
        public static bool TryDecode(ReadOnlyMemory<byte> errorData, [MaybeNullWhen(false)] out Panic parsedError)
        {
            if(!IsMatchingSignature(errorData.Span))
            {
                parsedError = null;
                return false;
            }

            parsedError = Decode(errorData);
            return true;
        }
    }
}
