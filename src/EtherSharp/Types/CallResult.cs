using EtherSharp.Common.Exceptions;

namespace EtherSharp.Types;

/// <summary>
/// Represents the typed outcome of a safe contract call.
/// </summary>
/// <typeparam name="T">The decoded success payload type.</typeparam>
public abstract record CallResult<T>
{
    /// <summary>
    /// Indicates execution succeeded and the returned bytes decoded successfully.
    /// </summary>
    /// <param name="Value">The decoded return value.</param>
    public sealed record Success(T Value) : CallResult<T>;

    /// <summary>
    /// Indicates execution reverted and returned raw revert data.
    /// </summary>
    /// <param name="Data">The EVM revert payload, if any.</param>
    public sealed record Reverted(ReadOnlyMemory<byte> Data) : CallResult<T>;

    /// <summary>
    /// Indicates execution succeeded but the returned bytes could not be decoded for the expected ABI.
    /// </summary>
    /// <param name="Data">The raw return bytes that failed to decode.</param>
    /// <param name="Exception">The parsing exception produced while decoding the return bytes.</param>
    public sealed record Malformed(ReadOnlyMemory<byte> Data, CallParsingException Exception) : CallResult<T>;

    /// <summary>
    /// Returns the decoded call value, or throws a parsed revert or decoding exception.
    /// </summary>
    /// <param name="callToAddress">The target contract address used for revert parsing.</param>
    /// <returns>The decoded call result.</returns>
    /// <exception cref="CallRevertedException">Thrown when this instance is a <see cref="Reverted"/> result.</exception>
    /// <exception cref="CallParsingException">Thrown when this instance is a <see cref="Malformed"/> result.</exception>
    public T Unwrap(Address? callToAddress) => this switch
    {
        Success s => s.Value,
        Reverted r => throw CallRevertedException.Parse(callToAddress, r.Data.Span),
        Malformed m => throw m.Exception,
        _ => throw new NotImplementedException()
    };

    internal static CallResult<T> ParseFrom(TxCallResult txCallResult, Func<ReadOnlyMemory<byte>, T> decoder)
        => txCallResult switch
        {
            TxCallResult.Success success => ParseSuccessFrom(success.Data, decoder),
            TxCallResult.Reverted reverted => new Reverted(reverted.Data),
            _ => throw new NotImplementedException()
        };

    internal static CallResult<T> ParseSuccessFrom(ReadOnlyMemory<byte> data, Func<ReadOnlyMemory<byte>, T> decoder)
    {
        try
        {
            return new Success(decoder(data));
        }
        catch(CallParsingException ex)
        {
            return new Malformed(data, ex);
        }
        catch(Exception ex)
        {
            return new Malformed(data, new CallParsingException.MalformedCallDataException(data, ex));
        }
    }
}
