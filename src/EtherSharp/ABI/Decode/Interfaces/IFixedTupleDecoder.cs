using EtherSharp.Types;

namespace EtherSharp.ABI.Decode.Interfaces;

/// <summary>
/// Interface for decoding fixed-size tuple elements from ABI-encoded data.
/// </summary>
public partial interface IFixedTupleDecoder
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
    /// Reads a sized bytes value from the input.
    /// </summary>
    /// <param name="bitLength">The bit length of the bytes value (e.g., 8 for bytes1, 256 for bytes32).</param>
    /// <returns>The decoded bytes value.</returns>
    public ReadOnlyMemory<byte> SizedBytes(int bitLength);

    /// <summary>
    /// Decodes a fixed-size tuple from the input.
    /// </summary>
    /// <typeparam name="T">The type of the tuple to decode.</typeparam>
    /// <param name="func">A function that decodes the tuple elements.</param>
    /// <returns>The decoded tuple.</returns>
    public T FixedTuple<T>(Func<IFixedTupleDecoder, T> func);
}
