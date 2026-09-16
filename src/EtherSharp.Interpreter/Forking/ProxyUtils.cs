using EtherSharp.Types;
using System.Collections.Frozen;

namespace EtherSharp.Interpreter.Forking;

internal static class ProxyUtils
{
    public static FrozenSet<Bytes32> CodeStorageSlots { get; } = new[]
    {
        // EIP-1967 implementation and beacon slots (keccak256 labels minus one).
        Bytes32.Parse("0x360894a13ba1a3210667c828492db98dca3e2076cc3735a920a3ca505d382bbc"),
        Bytes32.Parse("0xa3f0ad74e5423aebfd80d3ef4346578335a9a72aeaee59ff6cb3582b35133d50"),
        // ERC-1822 and legacy ZeppelinOS implementation slots.
        Bytes32.Parse("0xc5f16f0fcc639fa48a6947836d9850f504798523bf8c9a3a87d5876cf622bcf7"),
        Bytes32.Parse("0x7050c9e0f4ca769c69bd3a8ef740bc37934f8e2c036e5a723fd8ee048ed3f8c3"),
    }.ToFrozenSet();
}
