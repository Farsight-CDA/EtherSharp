namespace EtherSharp.Common.Exceptions;

/// <summary>
/// Thrown when the node returns a <c>null</c> result for an RPC method that requires a value.
/// </summary>
/// <param name="method">The JSON-RPC method that returned <c>null</c>.</param>
public sealed class RpcNullResultException(string method)
    : RpcException($"RPC method {method} unexpectedly returned null")
{
    /// <summary>
    /// The JSON-RPC method that returned <c>null</c>.
    /// </summary>
    public string Method { get; } = method;
}
