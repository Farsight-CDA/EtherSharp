using EtherSharp.ABI.Types;
using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;
/// <summary>
/// Base exception for <c>eth_call</c> responses that indicate a revert.
/// </summary>
/// <param name="callAddress">Contract address the call was sent to, when available.</param>
/// <param name="message">Human-readable revert description used to build the exception message.</param>
public abstract class CallRevertedException(Address? callAddress, string message)
    : Exception(callAddress is null ? $"Call {message}" : $"Call to {callAddress.String} {message}")
{
    private static ReadOnlySpan<byte> ErrorStringSignature => [0x08, 0xc3, 0x79, 0xa0];
    private static ReadOnlySpan<byte> PanicSignature => [0x4e, 0x48, 0x7b, 0x71];

    /// <summary>
    /// Parses revert bytes returned by a call into a specific <see cref="CallRevertedException"/> subtype.
    /// </summary>
    /// <param name="callAddress">Contract address the call targeted, when known.</param>
    /// <param name="data">Raw revert payload returned by the node.</param>
    /// <returns>A typed revert exception that captures the decoded payload.</returns>
    public static CallRevertedException Parse(Address? callAddress, ReadOnlySpan<byte> data)
    {
        if(data.Length == 0)
        {
            return new CallRevertedWithNoDataException(callAddress);
        }

        var errorSignature = data[0..4];

        if(errorSignature.SequenceEqual(ErrorStringSignature))
        {
            return new CallRevertedWithMessageException(
                callAddress, AbiTypes.String.Decode(data[4..], 0)
            );
        }
        else if(errorSignature.SequenceEqual(PanicSignature))
        {
            return new CallRevertedWithPanicException(
                callAddress, (PanicType) AbiTypes.Byte.Decode(data[4..])
            );
        }
        //
        return new CallRevertedWithCustomErrorException(callAddress, data.ToArray());
    }

    /// <summary>
    /// Address that the call was sent to.
    /// </summary>
    public Address? CallAddress { get; } = callAddress;

    /// <summary>
    /// Thrown when a call reverts without any error data.
    /// </summary>
    public class CallRevertedWithNoDataException(Address? callAddress)
        : CallRevertedException(callAddress, "reverted with no data, this likely means that the contract does not implement the called method.")
    {
    }

    /// <summary>
    /// Thrown when a call reverts with an error message.
    /// </summary>
    /// <param name="callAddress">Contract address the call targeted, when known.</param>
    /// <param name="message">Decoded Solidity <c>Error(string)</c> message.</param>
    public class CallRevertedWithMessageException(Address? callAddress, string message)
        : CallRevertedException(callAddress, message)
    {
        /// <summary>
        /// The error message returned by the contract.
        /// </summary>
        public string ContractErrorMessage { get; } = message;
    }

    /// <summary>
    /// Thrown when a call reverts with a custom error payload.
    /// </summary>
    /// <param name="callAddress">Contract address the call targeted, when known.</param>
    /// <param name="data">Raw revert bytes including selector and encoded arguments.</param>
    public class CallRevertedWithCustomErrorException(Address? callAddress, byte[] data)
        : CallRevertedException(callAddress, $"reverted with custom error: 0x{Convert.ToHexStringLower(data.AsSpan(0, 4))}")
    {
        /// <summary>
        /// The custom error data returned by the contract.
        /// </summary>
        public byte[] ContractErrorData { get; } = data;
    }

    /// <summary>
    /// Thrown when a call reverts with a panic.
    /// </summary>
    /// <param name="callAddress">Contract address the call targeted, when known.</param>
    /// <param name="type">Decoded Solidity panic reason.</param>
    public class CallRevertedWithPanicException(Address? callAddress, PanicType type)
        : CallRevertedException(callAddress, $"reverted with panic: {type}")
    {
        /// <summary>
        /// The panic type returned by the contract.
        /// </summary>
        public PanicType PanicType { get; } = type;
    }
}
