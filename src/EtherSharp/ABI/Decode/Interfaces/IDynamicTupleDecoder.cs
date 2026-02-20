using EtherSharp.Types;

namespace EtherSharp.ABI.Decode.Interfaces;

/// <summary>
/// Interface for decoding dynamic tuple elements from ABI-encoded data.
/// </summary>
public partial interface IDynamicTupleDecoder
{
    /// <summary>
    /// Reads a boolean value from the input.
    /// </summary>
    /// <returns>The decoded boolean value.</returns>
    public bool Bool();

    /// <summary>
    /// Reads an Ethereum address from the input.
    /// </summary>
    /// <returns>The decoded address.</returns>
    public Address Address();

    /// <summary>
    /// Reads a string value from the input.
    /// </summary>
    /// <returns>The decoded string.</returns>
    public string String();

    /// <summary>
    /// Reads a dynamic bytes value from the input.
    /// </summary>
    /// <returns>The decoded bytes.</returns>
    public ReadOnlyMemory<byte> Bytes();

    /// <summary>
    /// Reads a sized bytes value from the input.
    /// </summary>
    /// <param name="bitLength">The bit length of the bytes value (e.g., 8 for bytes1, 256 for bytes32).</param>
    /// <returns>The decoded bytes value.</returns>
    public ReadOnlyMemory<byte> SizedBytes(int bitLength);

    /// <summary>
    /// Reads a numeric value from the input.
    /// </summary>
    /// <typeparam name="TNumber">The numeric type to decode to.</typeparam>
    /// <param name="isUnsigned">Whether the number is unsigned.</param>
    /// <param name="bitLength">The bit length of the number.</param>
    /// <returns>The decoded numeric value.</returns>
    public TNumber Number<TNumber>(bool isUnsigned, int bitLength);

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
    /// Reads a dynamic bytes array from the input.
    /// </summary>
    /// <returns>The decoded bytes array.</returns>
    public ReadOnlyMemory<byte>[] BytesArray();

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
