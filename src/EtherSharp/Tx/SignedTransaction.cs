using EtherSharp.Types;

namespace EtherSharp.Tx;

/// <summary>
/// Contains an encoded signed transaction and its hash.
/// </summary>
/// <param name="EncodedTx">The encoded signed transaction represented as a hexadecimal string.</param>
/// <param name="Hash">The transaction hash.</param>
public readonly record struct SignedTransaction(string EncodedTx, Bytes32 Hash);
