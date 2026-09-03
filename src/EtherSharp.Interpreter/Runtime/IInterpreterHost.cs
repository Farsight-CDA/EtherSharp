using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Provides upstream EVM state and call execution at an interpreter context.
/// </summary>
public interface IInterpreterHost
{
    /// <summary>
    /// Gets the state for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account state, or <see langword="null"/> when the account does not exist.</returns>
    public Task<InterpreterAccountInfo?> GetAccountAsync(InterpreterContext context, Address address);

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
