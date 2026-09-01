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
    public sealed class EmptyReturnDataException() : CallParsingException("Call returned no data", null);

    /// <summary>
    /// Thrown when a decoder leaves unconsumed bytes in the returned data.
    /// </summary>
    /// <param name="returnData">Raw data returned by the call.</param>
    /// <param name="bytesConsumed">Number of bytes consumed by the decoder.</param>
    public sealed class RemainingReturnDataException(ReadOnlyMemory<byte> returnData, int bytesConsumed)
        : CallParsingException($"Call returned more data than expected, only consumed {bytesConsumed} out of {returnData.Length} bytes", null)
    {
        /// <summary>
        /// Raw return data from the call.
        /// </summary>
        public ReadOnlyMemory<byte> ReturnData { get; } = returnData;

        /// <summary>
        /// Number of bytes consumed by the decoder before it stopped.
        /// </summary>
        public int BytesConsumed { get; } = bytesConsumed;
    }

    /// <summary>
    /// Thrown when return data cannot be decoded for the expected ABI.
    /// </summary>
    /// <param name="returnData">Raw data returned by the call.</param>
    /// <param name="parsingException">Underlying exception produced by the decoder.</param>
    public sealed class MalformedReturnDataException(ReadOnlyMemory<byte> returnData, Exception parsingException)
        : CallParsingException("Call returned malformed data. Check your ABI", parsingException)
    {
        /// <summary>
        /// Raw return data from the call.
        /// </summary>
        public ReadOnlyMemory<byte> ReturnData { get; } = returnData;
    }
}
