using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Provides upstream EVM state and call execution at an interpreter context.
/// </summary>
public interface IInterpreterHost
{
    /// <summary>
    /// Gets the bytecode for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account bytecode.</returns>
    public Task<EVMByteCode> GetCodeAsync(InterpreterContext context, Address address);

    /// <summary>
    /// Gets the code hash for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account code hash.</returns>
    public Task<Bytes32> GetCodeHashAsync(InterpreterContext context, Address address);

    /// <summary>
    /// Gets the native balance for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account's native balance.</returns>
    public Task<UInt256> GetBalanceAsync(InterpreterContext context, Address address);

    /// <summary>
    /// Gets the transaction count (nonce) for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account's transaction count.</returns>
    public Task<ulong> GetNonceAsync(InterpreterContext context, Address address);

    /// <summary>
    /// Gets a persistent storage value for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <param name="key">The storage key.</param>
    /// <returns>The persistent storage value.</returns>
    public Task<Bytes32> GetStorageAtAsync(InterpreterContext context, Address address, Bytes32 key);

    /// <summary>
    /// Simulates a call against upstream state at the supplied interpreter context.
    /// </summary>
    /// <param name="context">The context at which the call is executed.</param>
    /// <param name="caller">The immediate message caller.</param>
    /// <param name="target">The account to call.</param>
    /// <param name="value">The native value supplied to the call.</param>
    /// <param name="input">The call input.</param>
    /// <returns>The raw call result.</returns>
    /// <remarks>State changes made by the upstream call are discarded and do not affect the interpreter journal.</remarks>
    public Task<TxCallResult> CallAsync(
        InterpreterContext context,
        Address caller,
        Address target,
        UInt256 value,
        ReadOnlyMemory<byte> input
    );
}
