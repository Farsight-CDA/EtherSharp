namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Base exception for failures while decoding successful call return data.
/// </summary>
/// <param name="message">Error message that describes the parsing failure.</param>
/// <param name="innerException">Underlying exception thrown during decoding, if any.</param>
public abstract class CallParsingException(string message, Exception? innerException) : Exception(message, innerException)
{
    /// <summary>
    /// Thrown when a call returns no bytes but return data was expected.
    /// </summary>
    public class EmptyCallDataException() : CallParsingException("Call returned no data", null);

    /// <summary>
    /// Thrown when a decoder leaves unconsumed bytes in the returned call data.
    /// </summary>
    /// <param name="calldata">Raw data returned by the call.</param>
    /// <param name="bytesConsumed">Number of bytes consumed by the decoder.</param>
    public class RemainingCallDataException(ReadOnlyMemory<byte> calldata, int bytesConsumed)
        : CallParsingException($"Call returned more data than expected, only consumed {bytesConsumed} out of {calldata.Length} bytes", null)
    {
        /// <summary>
        /// Raw return data from the call.
        /// </summary>
        public ReadOnlyMemory<byte> CallData { get; } = calldata;

        /// <summary>
        /// Number of bytes consumed by the decoder before it stopped.
        /// </summary>
        public int BytesConsumed { get; } = bytesConsumed;
    }

    /// <summary>
    /// Thrown when returned call data cannot be decoded for the expected ABI.
    /// </summary>
    /// <param name="calldata">Raw data returned by the call.</param>
    /// <param name="parsingException">Underlying exception produced by the decoder.</param>
    public class MalformedCallDataException(ReadOnlyMemory<byte> calldata, Exception parsingException)
        : CallParsingException("Call returned malformed data. Check your ABI", parsingException)
    {
        /// <summary>
        /// Raw return data from the call.
        /// </summary>
        public ReadOnlyMemory<byte> CallData { get; } = calldata;
    }
}
