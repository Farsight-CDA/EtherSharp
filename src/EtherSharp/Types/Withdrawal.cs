using EtherSharp.Numerics;
using System.Text.Json.Serialization;

namespace EtherSharp.Types;

public record Withdrawal(
    [property: JsonRequired] Address Address,
    [property: JsonRequired] UInt256 Amount,
    [property: JsonRequired] uint Index,
    [property: JsonRequired] uint ValidatorIndex
);
