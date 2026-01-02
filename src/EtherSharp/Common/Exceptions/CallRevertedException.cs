using EtherSharp.ABI.Types;
using EtherSharp.Types;

namespace EtherSharp.Common.Exceptions;
/// <summary>
/// Exception types thrown when eth_call reverts.
/// </summary>
/// <param name="callAddress"></param>
/// <param name="message"></param>
public abstract class CallRevertedException(Address? callAddress, string message)
    : Exception(callAddress is null ? $"Call {message}" : $"Call to {callAddress.String} {message}")
{
    private static ReadOnlySpan<byte> ErrorStringSignature => [0x08, 0xc3, 0x79, 0xa0];
    private static ReadOnlySpan<byte> PanicSignature => [0x4e, 0x48, 0x7b, 0x71];

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
    /// <param name="callAddress"></param>
    /// <param name="message"></param>
    public class CallRevertedWithMessageException(Address? callAddress, string message)
        : CallRevertedException(callAddress, message)
    {
        /// <summary>
        /// The error message returned by the contract.
        /// </summary>
        public string ContractErrorMessage { get; } = message;
    }

    /// <summary>
    /// /// Thrown when a call reverts with a custom error type.
    /// </summary>
    /// <param name="callAddress"></param>
    /// <param name="data"></param>
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
    /// <param name="callAddress"></param>
    /// <param name="type"></param>
    public class CallRevertedWithPanicException(Address? callAddress, PanicType type)
        : CallRevertedException(callAddress, $"reverted with panic: {type}")
    {
        /// <summary>
        /// The panic type returned by the contract.
        /// </summary>
        public PanicType PanicType { get; } = type;
    }
}
