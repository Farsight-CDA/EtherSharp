using EtherSharp.Common.Exceptions;

namespace EtherSharp.Types;

/// <summary>
/// Represents the typed outcome of a safe contract call.
/// </summary>
/// <param name="CallTarget">The call target address, or <see langword="null"/> for contract deployment.</param>
/// <typeparam name="T">The decoded success payload type.</typeparam>
public abstract record CallResult<T>(Address? CallTarget)
{
    /// <summary>
    /// Indicates execution succeeded and the returned bytes decoded successfully.
    /// </summary>
    /// <param name="CallTarget">The call target address, or <see langword="null"/> for contract deployment.</param>
    /// <param name="Value">The decoded return value.</param>
    public sealed record Success(Address? CallTarget, T Value) : CallResult<T>(CallTarget);

    /// <summary>
    /// Indicates execution reverted and returned raw revert data.
    /// </summary>
    /// <param name="CallTarget">The call target address, or <see langword="null"/> for contract deployment.</param>
    /// <param name="Data">The EVM revert payload, if any.</param>
    public sealed record Reverted(Address? CallTarget, ReadOnlyMemory<byte> Data) : CallResult<T>(CallTarget);

    /// <summary>
    /// Indicates execution succeeded but the returned bytes could not be decoded for the expected ABI.
    /// </summary>
    /// <param name="CallTarget">The call target address, or <see langword="null"/> for contract deployment.</param>
    /// <param name="Data">The raw return bytes that failed to decode.</param>
    /// <param name="Exception">The parsing exception produced while decoding the return bytes.</param>
    public sealed record Malformed(Address? CallTarget, ReadOnlyMemory<byte> Data, CallParsingException Exception) : CallResult<T>(CallTarget);

    /// <summary>
    /// Returns the decoded call value, or throws a parsed revert or decoding exception.
    /// </summary>
    /// <returns>The decoded call result.</returns>
    /// <exception cref="CallRevertedException">Thrown when this instance is a <see cref="Reverted"/> result.</exception>
    /// <exception cref="CallParsingException">Thrown when this instance is a <see cref="Malformed"/> result.</exception>
    public T Unwrap()
        => this switch
        {
            Success s => s.Value,
            Reverted r => throw CallRevertedException.Parse(CallTarget, r.Data.Span),
            Malformed m => throw m.Exception,
            _ => throw new NotImplementedException()
        };

    internal static CallResult<T> ParseFrom(TxCallResult txCallResult, Address? callTarget, Func<ReadOnlyMemory<byte>, T> decoder)
        => txCallResult.Success
            ? ParseSuccessFrom(txCallResult.Data, callTarget, decoder)
            : new Reverted(callTarget, txCallResult.Data);

    internal static CallResult<T> ParseSuccessFrom(ReadOnlyMemory<byte> data, Address? callTarget, Func<ReadOnlyMemory<byte>, T> decoder)
    {
        try
        {
            return new Success(callTarget, decoder(data));
        }
        catch(CallParsingException ex)
        {
            return new Malformed(callTarget, data, ex);
        }
        catch(Exception ex)
        {
            return new Malformed(callTarget, data, new CallParsingException.MalformedCallDataException(data, ex));
        }
    }
}
