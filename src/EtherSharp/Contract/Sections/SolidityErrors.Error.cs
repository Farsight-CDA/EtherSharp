using EtherSharp.ABI;
using EtherSharp.Types;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Text;

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
        /// Hex encoded error selector based on function signature: Error(string)
        /// </summary>
        public static string SelectorHex { get; } = "0x08c379a0";

        /// <summary>
        /// Parsed bytes4 error selector based on signature: Error(string)
        /// </summary>
        public static Bytes4 Selector { get; } = Bytes4.Parse(SelectorHex);

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
        /// Encodes this error as Solidity revert data.
        /// </summary>
        /// <returns>Error data including selector and ABI-encoded arguments.</returns>
        public byte[] Encode()
            => EncodeData(Message);

        /// <summary>
        /// Encodes Solidity <c>Error(string)</c> revert data.
        /// </summary>
        /// <param name="message">Decoded revert message.</param>
        /// <returns>Error data including selector and ABI-encoded arguments.</returns>
        public static byte[] EncodeData(string message)
        {
            ArgumentNullException.ThrowIfNull(message);

            int stringLength = Encoding.UTF8.GetByteCount(message);
            int paddedStringLength = (stringLength + 31) / 32 * 32;
            byte[] data = new byte[4 + 32 + 32 + paddedStringLength];
            Selector.CopyTo(data);

            var arguments = data.AsSpan(4);
            BinaryPrimitives.WriteUInt32BigEndian(arguments[28..32], 32);
            BinaryPrimitives.WriteUInt32BigEndian(arguments[60..64], (uint) stringLength);

            return Encoding.UTF8.TryGetBytes(message, arguments[64..], out _)
                ? data
                : throw new InvalidOperationException("Failed to write bytes");
        }

        /// <summary>
        /// Checks whether the error data starts with the Solidity <c>Error(string)</c> selector.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <returns><see langword="true" /> when the selector matches; otherwise <see langword="false" />.</returns>
        public static bool IsMatchingSelector(ReadOnlySpan<byte> errorData)
            => errorData.Length >= 4 && Selector == Bytes4.FromBytes(errorData[0..4]);

        /// <summary>
        /// Attempts to decode Solidity <c>Error(string)</c> revert data.
        /// </summary>
        /// <param name="errorData">Error data including selector and ABI-encoded arguments.</param>
        /// <param name="parsedError">The decoded error when successful.</param>
        /// <returns><see langword="true" /> when the selector matches and the error was decoded; otherwise <see langword="false" />.</returns>
        public static bool TryDecode(ReadOnlyMemory<byte> errorData, [MaybeNullWhen(false)] out Error parsedError)
        {
            if(!IsMatchingSelector(errorData.Span))
            {
                parsedError = null;
                return false;
            }

            parsedError = Decode(errorData);
            return true;
        }
    }
}
