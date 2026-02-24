using EtherSharp.Numerics;
namespace EtherSharp.Types;

/// <summary>
/// Represents a block with its identifying fields and transaction hashes.
/// </summary>
/// <param name="Number">The block height.</param>
/// <param name="Timestamp">The timestamp when the block was produced.</param>
/// <param name="BaseFeePerGas">The base fee per gas unit for EIP-1559 blocks.</param>
/// <param name="Hash">The block hash.</param>
/// <param name="ParentHash">The parent block hash.</param>
/// <param name="Transactions">The transaction hashes included in the block.</param>
public record Block(
    ulong Number,
    DateTimeOffset Timestamp,
    UInt256? BaseFeePerGas,
    string Hash,
    string ParentHash,
    IReadOnlyList<string> Transactions
);
