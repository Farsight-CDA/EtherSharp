namespace EtherSharp.ABI.Encode.Interfaces;

/// <summary>
/// Interface for encoding array elements to ABI-encoded data.
/// </summary>
public partial interface IArrayAbiEncoder
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
    /// Encodes an array of complex elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="values">The elements to encode.</param>
    /// <param name="func">An action that encodes each element.</param>
    public void Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func);

    /// <summary>
    /// Encodes a dynamic tuple.
    /// </summary>
    /// <param name="func">An action that encodes the tuple elements.</param>
    public void DynamicTuple(Action<IDynamicTupleEncoder> func);

    /// <summary>
    /// Encodes a fixed-size tuple.
    /// </summary>
    /// <param name="func">An action that encodes the tuple elements.</param>
    public void FixedTuple(Action<IFixedTupleEncoder> func);

    /// <summary>
    /// Attempts to write the encoded data to the output buffer.
    /// </summary>
    /// <param name="outputBuffer">The buffer to write to.</param>
    /// <returns>True if the write was successful; otherwise, false.</returns>
    internal bool TryWriteTo(Span<byte> outputBuffer);
}
