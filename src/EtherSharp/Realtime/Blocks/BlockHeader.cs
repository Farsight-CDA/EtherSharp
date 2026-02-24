using EtherSharp.Types;

namespace EtherSharp.Realtime.Blocks;

/// <summary>
/// Represents core metadata for a block.
/// </summary>
/// <param name="Number">The block height.</param>
/// <param name="Timestamp">The timestamp of when the block was produced.</param>
/// <param name="Hash">The unique hash of the block.</param>
/// <param name="ParentHash">The hash of the parent block.</param>
/// <param name="Nonce">The nonce used in block production.</param>
/// <param name="Miner">The address of the block producer.</param>
/// <param name="GasLimit">The maximum amount of gas allowed in the block.</param>
/// <param name="GasUsed">The amount of gas consumed by transactions in the block.</param>
public record BlockHeader(
    ulong Number,
    DateTimeOffset Timestamp,
    string Hash,
    string ParentHash,
    ulong Nonce,
    Address Miner,
    ulong GasLimit,
    ulong GasUsed
);
