using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;
/// <summary>
/// Exception types thrown when eth_call reverts.
/// </summary>
/// <param name="message"></param>
public class CallRevertedException(string message) : Exception(message)
{
    /// <summary>
    /// Thrown when a call reverts with an error message.
    /// </summary>
    /// <param name="message"></param>
    public class CallRevertedWithMessageException(string message)
        : CallRevertedException(message)
    {
        /// <summary>
        /// The error message returned by the contract.
        /// </summary>
        public string ContractErrorMessage { get; } = message;
    }

    /// <summary>
    /// /// Thrown when a call reverts with a custom error type.
    /// </summary>
    /// <param name="data"></param>
    public class CallRevertedWithCustomErrorException(byte[] data)
        : CallRevertedException($"Custom Error Signature: 0x{Convert.ToHexStringLower(data.AsSpan(0, 4))}")
    {
        /// <summary>
        /// The custom error data returned by the contract.
        /// </summary>
        public byte[] ContractErrorData { get; } = data;
    }

    /// <summary>
    /// Thrown when a call reverts with a panic.
    /// </summary>
    /// <param name="type"></param>
    public class CallRevertedWithPanicException(PanicType type)
        : CallRevertedException($"Call Reverted with Panic: {type}")
    {
        /// <summary>
        /// The panic type returned by the contract.
        /// </summary>
        public PanicType PanicType { get; } = type;
    }
}
