using EtherSharp.Contract;
using EtherSharp.Crypto;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Represents the state of an existing EVM account.
/// </summary>
/// <param name="Balance">The account's native balance.</param>
/// <param name="Nonce">The account nonce.</param>
/// <param name="CodeHash">
/// The account's canonical code hash. Accounts without code use the Keccak-256 hash of empty data.
/// </param>
/// <param name="Code">The account bytecode.</param>
public readonly record struct InterpreterAccountInfo(
    UInt256 Balance,
    ulong Nonce,
    Bytes32 CodeHash,
    EVMByteCode Code
)
{
    /// <summary>
    /// Gets the canonical code hash for an account without code.
    /// </summary>
    public static Bytes32 EmptyCodeHash { get; } = Keccak256.HashData([]);

    /// <summary>
    /// Gets the state defaults for a missing account.
    /// </summary>
    public static InterpreterAccountInfo Empty { get; } = new(
        UInt256.Zero,
        0,
        EmptyCodeHash,
        new EVMByteCode(ReadOnlyMemory<byte>.Empty)
    );
}
