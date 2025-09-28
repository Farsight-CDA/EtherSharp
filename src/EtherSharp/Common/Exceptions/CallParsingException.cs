namespace EtherSharp.Common.Exceptions;

public abstract class CallParsingException(string message, Exception? innerException) : Exception(message, innerException)
{
    public class EmptyCallDataException() : CallParsingException("Call returned no data", null);
    public class RemainingCallDataException(byte[] calldata) : CallParsingException("Call returned more data than expected", null)
    {
        public byte[] CallData { get; } = calldata;
    }
    public class MalformedCallDataException(byte[] calldata, Exception parsingException) : CallParsingException("Call returned malformed data. Check your ABI", parsingException)
    {
        public byte[] CallData { get; } = calldata;
    }
}