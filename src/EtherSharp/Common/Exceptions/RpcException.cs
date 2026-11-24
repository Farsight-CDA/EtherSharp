namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Base type for all failed RPC calls.
/// </summary>
public closed class RpcException : Exception
{
    private protected RpcException(string? message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
