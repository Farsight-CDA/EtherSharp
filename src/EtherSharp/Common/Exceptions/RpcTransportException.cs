namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Thrown when no usable JSON-RPC response was received, e.g. on timeouts, disconnects or malformed responses.
/// </summary>
public sealed class RpcTransportException : RpcException
{
    /// <summary>
    /// Initializes a new transport exception with a message.
    /// </summary>
    /// <param name="message">Transport failure description.</param>
    public RpcTransportException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new transport exception with a message and root cause.
    /// </summary>
    /// <param name="message">Transport failure description.</param>
    /// <param name="innerException">Underlying exception thrown by the transport implementation.</param>
    public RpcTransportException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
