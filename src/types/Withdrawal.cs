using System.Text.Json.Serialization;

namespace EVM.net.types;

public record Withdrawal([property: JsonRequired] string Address, [property: JsonRequired] uint Amount, [property: JsonRequired] uint Index, [property: JsonRequired] uint ValidatorIndex);