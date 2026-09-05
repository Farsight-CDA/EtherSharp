using EtherSharp.Contract;
using EtherSharp.Numerics;
using EtherSharp.Types;

namespace EtherSharp.Interpreter.Runtime;

/// <summary>
/// Provides upstream EVM state and call execution for an interpreter state fork.
/// </summary>
/// <remarks>Each host instance represents one interpreter's registration.</remarks>
public interface IInterpreterHost
{
    /// <summary>
    /// Unregisters this interpreter from its host. Repeated calls have no effect.
    /// </summary>
    /// <remarks>Must not be called while an interpreter operation is in progress.</remarks>
    public void Unregister();

    /// <summary>
    /// Gets an account's native balance.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <returns>The account balance.</returns>
    public ValueTask<UInt256> GetBalanceAsync(Address address);

    /// <summary>
    /// Gets an account's nonce.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <returns>The account nonce.</returns>
    public ValueTask<ulong> GetNonceAsync(Address address);

    /// <summary>
    /// Gets an account's bytecode.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <returns>The account bytecode.</returns>
    public ValueTask<EVMByteCode> GetCodeAsync(Address address);

    /// <summary>
    /// Gets an account's external code hash.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <returns>
    /// The canonical code hash, or <see langword="null"/> when the account does not exist or is empty according to EIP-161.
    /// </returns>
    public ValueTask<Bytes32?> GetCodeHashAsync(Address address);

    /// <summary>
    /// Gets a persistent storage value for an account.
    /// </summary>
    /// <param name="address">The account address.</param>
    /// <param name="key">The storage key.</param>
    /// <returns>The persistent storage value.</returns>
    public ValueTask<Bytes32> GetStorageAtAsync(Address address, Bytes32 key);

    /// <summary>
    /// Executes an input-only precompile against upstream state at the interpreter state fork's context.
    /// </summary>
    /// <param name="caller">The immediate message caller.</param>
    /// <param name="target">The account to call.</param>
    /// <param name="value">The native value supplied to the call.</param>
    /// <param name="input">The call input. Its backing memory must remain unchanged until the returned task completes.</param>
    /// <returns>The raw call result.</returns>
    /// <remarks>
    /// This operation is not for arbitrary contract calls or stateful precompiles. Results must not depend
    /// on contract code or storage. Upstream state changes are discarded and do not affect the interpreter journal.
    /// </remarks>
    public Task<TxCallResult> CallPrecompileAsync(
        Address caller,
        Address target,
        UInt256 value,
        ReadOnlyMemory<byte> input
    );
}
