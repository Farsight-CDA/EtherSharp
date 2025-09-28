namespace EtherSharp.Common.Exceptions;

public abstract class CallParsingException(string message, Exception? innerException) : Exception(message, innerException)
{
    public class EmptyCallDataException() : CallParsingException("Call returned no data", null);
    public class RemainingCallDataException(ReadOnlyMemory<byte> calldata, int bytesConsumed)
        : CallParsingException($"Call returned more data than expected, only consumed {bytesConsumed} out of {calldata.Length} bytes", null)
    {
        public ReadOnlyMemory<byte> CallData { get; } = calldata;
        public int BytesConsumed { get; } = bytesConsumed;
    }
    public class MalformedCallDataException(ReadOnlyMemory<byte> calldata, Exception parsingException)
        : CallParsingException("Call returned malformed data. Check your ABI", parsingException)
    {
        public ReadOnlyMemory<byte> CallData { get; } = calldata;
    }
}