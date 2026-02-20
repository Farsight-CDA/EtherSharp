namespace EtherSharp.Common.Exceptions;

public class RPCTransportException : Exception
{
    public RPCTransportException(string message)
        : base(message)
    {
    }

    public RPCTransportException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
