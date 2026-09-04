using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Describes one logical interpreter data request.
/// </summary>
public abstract record InterpreterDataRequest
{
    private InterpreterDataRequest()
    {
    }

    /// <summary>
    /// Requests an account's native balance.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Balance(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's nonce.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Nonce(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's bytecode.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record Code(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests an account's canonical code hash.
    /// </summary>
    /// <param name="Address">The account address.</param>
    public sealed record CodeHash(Address Address) : InterpreterDataRequest;

    /// <summary>
    /// Requests one persistent storage slot.
    /// </summary>
    /// <param name="Address">The account address.</param>
    /// <param name="Key">The storage key.</param>
    public sealed record Storage(Address Address, Bytes32 Key) : InterpreterDataRequest;

    /// <summary>
    /// Requests execution of an upstream call whose state changes are discarded.
    /// </summary>
    /// <param name="Caller">The immediate message caller.</param>
    /// <param name="Target">The account to call.</param>
    /// <param name="Value">The native value supplied to the call.</param>
    /// <param name="Input">The call input.</param>
    public sealed record Call(
        Address Caller,
        Address Target,
        UInt256 Value,
        ReadOnlyMemory<byte> Input
    ) : InterpreterDataRequest;
}
