using EtherSharp.Types;

namespace EtherSharp.ABI.Decode.Interfaces;

/// <summary>
/// Interface for decodingAn array elements from ABI-encoded data.
/// </summary>
public partial interface IArrayAbiDecoder
{
    /// <summary>
    /// Reads a boolean array from the input.
    /// </summary>
    /// <returns>The decoded boolean array.</returns>
    public bool[] BoolArray();

    /// <summary>
    /// Reads an address array from the input.
    /// </summary>
    /// <returns>The decoded address array.</returns>
    public Address[] AddressArray();

    /// <summary>
    /// Reads a numeric array from the input.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type to decode to.</typeparam>
    /// <param name="isUnsigned">Whether the numbers are unsigned.</param>
    /// <param name="bitLength">The bit length of each number.</param>
    /// <returns>The decoded numeric array.</returns>
    public TNumber[] NumberArray<TNumber>(bool isUnsigned, uint bitLength);

    /// <summary>
    /// Reads a string array from the input.
    /// </summary>
    /// <returns>The decoded string array.</returns>
    public string[] StringArray();

    /// <summary>
    /// Reads an array of complex elements from the input.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="func">A function that decodes each element.</param>
    /// <returns>The decoded array.</returns>
    public T[] Array<T>(Func<IArrayAbiDecoder, T> func);

    /// <summary>
    /// Decodes a fixed-size tuple from the input.
    /// </summary>
    /// <typeparam name="T">The type of the tuple to decode.</typeparam>
    /// <param name="func">A function that decodes the tuple elements.</param>
    /// <returns>The decoded tuple.</returns>
    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);

    /// <summary>
    /// Decodes a dynamic tuple from the input.
    /// </summary>
    /// <typeparam name="T">The type of the tuple to decode.</typeparam>
    /// <param name="func">A function that decodes the tuple elements.</param>
    /// <returns>The decoded tuple.</returns>
    public T DynamicTuple<T>(Func<IDynamicTupleDecoder, T> func);
}
