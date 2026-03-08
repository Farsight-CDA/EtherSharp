using EtherSharp.ABI;
using EtherSharp.Numerics;

namespace EtherSharp.Tx;

/// <summary>
/// Represents a flash-call payload executed against an ephemeral deployment.
/// </summary>
public interface IFlashCall
{
    /// <summary>
    /// The ETH value supplied to the flash call.
    /// </summary>
    public UInt256 Value { get; }

    /// <summary>
    /// The calldata of the flash call.
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; }

    /// <summary>
    /// Creates an <see cref="IFlashCall"/> for a flash call that returns no decoded result.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <returns></returns>
    public static IFlashCall ForFlashCall(UInt256 value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWriteTo(data.AsSpan()[functionSignature.Length..]);
        return new FlashCallInput(value, data);
    }

    /// <summary>
    /// Creates an <see cref="IFlashCall{T}"/> for a raw undecoded flash call.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static IFlashCall<ReadOnlyMemory<byte>> ForRawFlashCall(UInt256 value, ReadOnlyMemory<byte> data)
        => new FlashCallInput<ReadOnlyMemory<byte>>(value, data, x => x);

    /// <summary>
    /// Creates an <see cref="IFlashCall{T}"/> for a raw flash call with a custom decoder.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="data"></param>
    /// <param name="decodeFunc"></param>
    /// <returns></returns>
    public static IFlashCall<T> ForRawFlashCall<T>(UInt256 value, ReadOnlyMemory<byte> data, Func<ReadOnlyMemory<byte>, T> decodeFunc)
        => new FlashCallInput<T>(value, data, decodeFunc);
}

/// <summary>
/// Represents a flash-call payload with a decoded return value.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IFlashCall<T> : IFlashCall
{
    /// <summary>
    /// Parses the result of type <typeparamref name="T"/> from the given call result data.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public T ReadResultFrom(ReadOnlyMemory<byte> data);

    /// <summary>
    /// Creates an <see cref="IFlashCall{T}"/> for a flash call that returns a result of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="functionSignature"></param>
    /// <param name="encoder"></param>
    /// <param name="decoder"></param>
    /// <returns></returns>
    public static IFlashCall<T> ForFlashCall(UInt256 value, ReadOnlyMemory<byte> functionSignature, AbiEncoder encoder, Func<AbiDecoder, T> decoder)
    {
        byte[] data = new byte[functionSignature.Length + encoder.Size];
        functionSignature.CopyTo(data);
        encoder.TryWriteTo(data.AsSpan()[functionSignature.Length..]);
        return new FlashCallInput<T>(value, data, x => decoder(new AbiDecoder(x)));
    }
}
