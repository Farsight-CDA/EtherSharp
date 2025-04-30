using System.Text.Json.Serialization;

namespace EtherSharp.Types;

public record Withdrawal(
    [property: JsonRequired] string Address,
    [property: JsonRequired] uint Amount,
    [property: JsonRequired] uint Index,
    [property: JsonRequired] uint ValidatorIndex
);