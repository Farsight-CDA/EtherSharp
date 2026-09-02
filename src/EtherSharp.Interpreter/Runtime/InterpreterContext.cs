using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Represents the transaction and block environment for interpreter execution.
/// </summary>
/// <param name="ChainId">The chain identifier.</param>
/// <param name="BlockNumber">The current block number.</param>
/// <param name="BlockTimestamp">The current block timestamp.</param>
/// <param name="RecentBlockHashes">Recent block hashes ordered from the parent block backwards.</param>
/// <param name="GasPrice">The effective transaction gas price.</param>
/// <param name="BaseFee">The block base fee, or <see langword="null"/> when <c>BASEFEE</c> is unsupported.</param>
/// <param name="BlobBaseFee">
/// The blob base fee, or <see langword="null"/> when blob opcodes are unsupported.
/// </param>
/// <param name="Coinbase">The block fee recipient.</param>
/// <param name="PrevRandao">The previous block randomness value.</param>
/// <param name="GasLimit">The block gas limit.</param>
public record InterpreterContext(
    ulong ChainId,
    ulong BlockNumber,
    DateTimeOffset BlockTimestamp,
    Bytes32[] RecentBlockHashes,
    UInt256 GasPrice,
    UInt256? BaseFee,
    UInt256? BlobBaseFee,
    Address Coinbase,
    UInt256 PrevRandao,
    UInt256 GasLimit
);
