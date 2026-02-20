using EtherSharp.Types;

namespace EtherSharp.ABI.Encode.Interfaces;

/// <summary>
/// Interface for encoding dynamic tuple elements to ABI-encoded data.
/// </summary>
public partial interface IDynamicTupleEncoder
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
    /// Attempts to write the encoded data to the output buffer.
    /// </summary>
    /// <param name="outputBuffer">The buffer to write to.</param>
    /// <returns>True if the write was successful; otherwise, false.</returns>
    internal bool TryWritoTo(Span<byte> outputBuffer);

    /// <summary>
    /// Encodes a boolean value.
    /// </summary>
    /// <param name="value">The boolean value to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Bool(bool value);

    /// <summary>
    /// Encodes an Ethereum address.
    /// </summary>
    /// <param name="value">The address to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Address(Address value);

    /// <summary>
    /// Encodes a string value.
    /// </summary>
    /// <param name="value">The string to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder String(string value);

    /// <summary>
    /// Encodes a dynamic bytes value.
    /// </summary>
    /// <param name="value">The bytes to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Bytes(ReadOnlyMemory<byte> value);

    /// <summary>
    /// Encodes a boolean array.
    /// </summary>
    /// <param name="values">The boolean values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder BoolArray(params bool[] values);

    /// <summary>
    /// Encodes an address array.
    /// </summary>
    /// <param name="addresses">The addresses to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder AddressArray(params Address[] addresses);

    /// <summary>
    /// Encodes a string array.
    /// </summary>
    /// <param name="values">The strings to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder StringArray(params string[] values);

    /// <summary>
    /// Encodes a dynamic bytes array.
    /// </summary>
    /// <param name="values">The bytes values to encode.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder BytesArray(params ReadOnlyMemory<byte>[] values);

    /// <summary>
    /// Encodes an array of complex elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="values">The elements to encode.</param>
    /// <param name="func">An action that encodes each element.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder Array<T>(IEnumerable<T> values, Action<IArrayAbiEncoder, T> func);

    /// <summary>
    /// Encodes a fixed-size tuple.
    /// </summary>
    /// <param name="func">An action that encodes the tuple elements.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder FixedTuple(Action<IFixedTupleEncoder> func);

    /// <summary>
    /// Encodes a dynamic tuple.
    /// </summary>
    /// <param name="func">An action that encodes the tuple elements.</param>
    /// <returns>This encoder instance for method chaining.</returns>
    public IDynamicTupleEncoder DynamicTuple(Action<IDynamicTupleEncoder> func);
}
