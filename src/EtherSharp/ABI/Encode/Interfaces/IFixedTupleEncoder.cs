using EtherSharp.Types;

namespace EtherSharp.ABI.Encode.Interfaces;

/// <summary>
/// Interface for encoding fixed-size tuple elements to ABI-encoded data.
/// </summary>
public partial interface IFixedTupleEncoder
{
    /// <summary>
    /// Gets the size of the metadata in bytes.
    /// </summary>
    public int MetadataSize { get; }

    /// <summary>
    /// Gets the size of the payload in bytes.
    /// </summary>
    public int PayloadSize { get; }

    /// <summary>
    /// Encodes a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Bool(bool value);

    /// <summary>
    /// Encodes an Ethereum address.
    /// </summary>
    /// <param name="value">The address to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder Address(Address value);

    /// <summary>
    /// Encodes a fixed-size tuple.
    /// </summary>
    /// <param name="func">An action that encodes the tuple elements.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IFixedTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);

    /// <summary>
    /// Attempts to write the encoded data to the output buffer.
    /// </summary>
    /// <param name="outputBuffer">The buffer to write to.</param>
    /// <returns>True if the write was successful; otherwise, false.</returns>
    internal bool TryWritoTo(Span<byte> outputBuffer);
}
