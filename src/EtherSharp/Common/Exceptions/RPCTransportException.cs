namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Represents a transport-level RPC failure (for example timeout, disconnect, or protocol I/O failure).
/// </summary>
public class RPCTransportException : Exception
{
    /// <summary>
    /// Initializes a new transport exception with a message.
    /// </summary>
    /// <param name="message">Transport failure description.</param>
    public RPCTransportException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new transport exception with a message and root cause.
    /// </summary>
    /// <param name="message">Transport failure description.</param>
    /// <param name="innerException">Underlying exception thrown by the transport implementation.</param>
    public RPCTransportException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
