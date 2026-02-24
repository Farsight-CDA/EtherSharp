using EtherSharp.Numerics;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;

/// <summary>
/// Represents a consensus-layer withdrawal included in a block.
/// </summary>
/// <param name="Address">The recipient address of the withdrawal.</param>
/// <param name="Amount">The withdrawn amount.</param>
/// <param name="Index">The withdrawal index within the chain.</param>
/// <param name="ValidatorIndex">The validator index associated with the withdrawal.</param>
public record Withdrawal(
    [property: JsonRequired] Address Address,
    [property: JsonRequired] UInt256 Amount,
    [property: JsonRequired] ulong Index,
    [property: JsonRequired] ulong ValidatorIndex
);
