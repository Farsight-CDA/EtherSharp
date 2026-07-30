using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Describes block-context fields overridden during an <c>eth_call</c> or <c>eth_estimateGas</c> simulation.
/// </summary>
/// <param name="Number">Simulated block number.</param>
/// <param name="Time">Simulated block timestamp.</param>
/// <param name="GasLimit">Simulated block gas limit.</param>
/// <param name="FeeRecipient">Simulated fee recipient.</param>
/// <param name="BaseFeePerGas">Simulated base fee per gas.</param>
public sealed record BlockOverride(
    ulong? Number = null,
    ulong? Time = null,
    ulong? GasLimit = null,
    Address? FeeRecipient = null,
    UInt256? BaseFeePerGas = null
);
