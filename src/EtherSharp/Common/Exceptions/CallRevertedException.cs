using EtherSharp.Common;
using EtherSharp.Types;
using SolidityError = EtherSharp.Contract.Sections.SolidityErrors.Error;
using SolidityPanic = EtherSharp.Contract.Sections.SolidityErrors.Panic;

namespace EtherSharp.Common.Exceptions;
/// <summary>
/// Base exception for <c>eth_call</c> responses that indicate a revert.
/// </summary>
/// <param name="callAddress">Contract address the call was sent to, when available.</param>
/// <param name="message">Human-readable revert description used to build the exception message.</param>
public abstract class CallRevertedException(Address? callAddress, string message)
    : Exception(message)
{
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

        if(data.Length < 4)
        {
            return new CallRevertedWithCustomErrorException(callAddress, data.ToArray());
        }

        byte[] errorData = data.ToArray();

        return SolidityError.TryDecode(errorData, out var error)
            ? new CallRevertedWithMessageException(callAddress, error.Message)
            : SolidityPanic.TryDecode(errorData, out var panic)
                ? new CallRevertedWithPanicException(callAddress, panic)
                : new CallRevertedWithCustomErrorException(callAddress, errorData);
    }

    /// <summary>
    /// Address that the call was sent to.
    /// </summary>
    public Address? CallAddress { get; } = callAddress;

    /// <summary>
    /// Thrown when a call reverts without any error data.
    /// </summary>
    public sealed class CallRevertedWithNoDataException(Address? callAddress)
        : CallRevertedException(callAddress,
            $"Contract call{(callAddress is { } address ? $" to {address}" : "")} reverted: no revert data was returned.")
    {
    }

    /// <summary>
    /// Thrown when a call reverts with an error message.
    /// </summary>
    /// <param name="callAddress">Contract address the call targeted, when known.</param>
    /// <param name="message">Decoded Solidity <c>Error(string)</c> message.</param>
    public sealed class CallRevertedWithMessageException(Address? callAddress, string message) : CallRevertedException(callAddress,
            $"Contract call{(callAddress is { } address ? $" to {address}" : "")} reverted: Solidity Error(string) returned '{message}'.")
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
    public sealed class CallRevertedWithCustomErrorException(Address? callAddress, byte[] data)
        : CallRevertedException(callAddress,
            $"Contract call{(callAddress is { } address ? $" to {address}" : "")} reverted with custom error {HexUtils.ToPrefixedHexString(data.AsSpan(0, 4))}. Revert data: {HexUtils.ToPrefixedHexString(data)}.")
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
    /// <param name="panic">Decoded Solidity panic error.</param>
    public sealed class CallRevertedWithPanicException(Address? callAddress, SolidityPanic panic) : CallRevertedException(callAddress,
            $"Contract call{(callAddress is { } address ? $" to {address}" : "")} reverted: {FormatPanicMessage(panic)}.")
    {
        /// <summary>
        /// The panic error returned by the contract.
        /// </summary>
        public SolidityPanic Panic { get; } = panic;

        /// <summary>
        /// The panic type returned by the contract.
        /// </summary>
        public PanicType PanicType { get; } = panic.Type;

        private static string FormatPanicMessage(SolidityPanic panic)
            => $"Solidity Panic(0x{(byte) panic.Type:x2}) {panic.Type}";
    }
}
