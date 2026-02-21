namespace EtherSharp.RPC;

/// <summary>
/// Represents the outcome of a JSON-RPC call as a discriminated union.
/// </summary>
/// <typeparam name="T">Expected type of the JSON-RPC <c>result</c> payload.</typeparam>
/// <remarks>
/// Consumers typically pattern match this value and handle <see cref="Success"/>, <see cref="Error"/>,
/// and <see cref="Null"/> explicitly. A <see cref="Null"/> instance means the node returned a successful
/// response with a <c>null</c> result payload.
/// </remarks>
public abstract record RpcResult<T>
{
    /// <summary>
    /// Indicates the RPC call completed without a protocol error, but returned a <c>null</c> result payload.
    /// </summary>
    public record Null() : RpcResult<T>;

    /// <summary>
    /// Indicates the RPC call completed successfully and contains the typed result payload.
    /// </summary>
    /// <param name="Result">Deserialized value of the JSON-RPC <c>result</c> field.</param>
    public record Success(T Result) : RpcResult<T>;

    /// <summary>
    /// Indicates the node returned a JSON-RPC error object.
    /// </summary>
    /// <param name="Code">JSON-RPC error code.</param>
    /// <param name="Message">JSON-RPC error message.</param>
    /// <param name="Data">Optional JSON-RPC error data payload, when provided by the node.</param>
    public record Error(int Code, string Message, string? Data) : RpcResult<T>;
}
