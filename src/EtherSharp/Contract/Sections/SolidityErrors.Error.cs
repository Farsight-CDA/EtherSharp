using EtherSharp.ABI;
using EtherSharp.Types;
using System.Diagnostics.CodeAnalysis;

namespace EtherSharp.Contract.Sections;

public static partial class SolidityErrors
{
    /// <summary>
    /// Built-in Solidity <c>Error(string)</c> revert error.
    /// </summary>
    /// <param name="Message">Decoded revert message.</param>
    public sealed record Error(string Message) : ISolidityError<Error>
    {
        /// <summary>
        /// Error signature used to calculate the signature bytes.
        /// </summary>
        public static string ErrorSignature { get; } = "Error(string)";

        /// <summary>
        /// Hex encoded error signature bytes based on function signature: Error(string)
        /// </summary>
        public static string SignatureHex { get; } = "0x08c379a0";

        /// <summary>
        /// Parsed bytes4 error selector based on signature: Error(string)
        /// </summary>
        public static Bytes4 Signature { get; } = Bytes4.Parse(SignatureHex);

        /// <summary>
        /// Decodes Solidity <c>Error(string)</c> revert data.
        /// </summary>
        /// <param name="data">Error data including selector and ABI-encoded arguments.</param>
        /// <returns>The decoded error.</returns>
        public static Error Decode(ReadOnlyMemory<byte> data)
        {
            var decoder = new AbiDecoder(data[4..]);
            return new Error(decoder.String());
        }

        /// <summary>
        /// Checks whether the error data starts with the Solidity <c>Error(string)</c> selector.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <returns><see langword="true" /> when the selector matches; otherwise <see langword="false" />.</returns>
        public static bool IsMatchingSignature(ReadOnlySpan<byte> errorData)
            => errorData.Length >= 4 && Signature == Bytes4.FromBytes(errorData[0..4]);

        /// <summary>
        /// Attempts to decode Solidity <c>Error(string)</c> revert data.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <param name="parsedError">The decoded error when successful.</param>
        /// <returns><see langword="true" /> when the selector matches and the error was decoded; otherwise <see langword="false" />.</returns>
        public static bool TryDecode(ReadOnlyMemory<byte> errorData, [MaybeNullWhen(false)] out Error parsedError)
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
