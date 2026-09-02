using EtherSharp.Contract;
using EtherSharp.Interpreter.Runtime;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Storage;

/// <summary>
/// Provides global EVM state at an interpreter context.
/// </summary>
public interface IGlobalStateProvider
{
    /// <summary>
    /// Gets the bytecode and code hash for an account.
    /// </summary>
    /// <param name="context">The context at which state is read.</param>
    /// <param name="address">The account address.</param>
    /// <returns>The account bytecode and its state hash.</returns>
    public Task<AccountCode> GetAccountCodeAsync(InterpreterContext context, Address address);

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
}
