using System.Numerics;

namespace EtherSharp.StateOverride;
public record OverrideAccount(
    ulong? Nonce,
    byte[]? Code,
    BigInteger? Balance,
    Dictionary<string, string>? State,
    Dictionary<string, string>? StateDiff,
    string? MovePrecompileToAddress
);
