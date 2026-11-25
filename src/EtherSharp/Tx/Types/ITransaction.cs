using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Wallet;

namespace EtherSharp.Tx.Types;

/// <summary>
/// Represents the common fields of an unsigned transaction payload.
/// </summary>
public interface ITransaction
{
    /// <summary>Gets the target chain identifier.</summary>
    public ulong ChainId { get; }
    /// <summary>Gets the transaction gas limit.</summary>
    public ulong Gas { get; }
    /// <summary>Gets the sender nonce.</summary>
    public ulong Nonce { get; }
    /// <summary>Gets the transaction destination, value, and calldata.</summary>
    public ITxInput Input { get; }

    /// <summary>
    /// Encodes and signs this transaction.
    /// </summary>
    /// <param name="signer">Signer of the transaction.</param>
    /// <param name="cancellationToken">Token used to cancel the signing operation.</param>
    /// <returns>The encoded signed transaction and its hash.</returns>
    public ValueTask<SignedTransaction> SignAsync(
        IEtherSigner signer,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Represents a transaction payload type and its associated tx-parameter and gas-parameter types.
/// </summary>
/// <typeparam name="TSelf">Concrete transaction payload type.</typeparam>
/// <typeparam name="TTxParams">Concrete transaction parameter type.</typeparam>
/// <typeparam name="TTxGasParams">Concrete gas parameter type.</typeparam>
public interface ITransaction<TSelf, TTxParams, TTxGasParams>
    : ITransaction
    where TTxParams : ITxParams<TTxParams>
    where TTxGasParams : ITxGasParams
    where TSelf : ITransaction<TSelf, TTxParams, TTxGasParams>
{
    /// <summary>
    /// Creates an unsigned transaction from its input and resolved parameters.
    /// </summary>
    /// <param name="chainId">Target chain identifier.</param>
    /// <param name="nonce">Sender nonce.</param>
    /// <param name="input">Transaction destination, value, and calldata.</param>
    /// <param name="txParams">Transaction-type-specific parameters.</param>
    /// <param name="gasParams">Gas limit and fee parameters.</param>
    /// <returns>The unsigned transaction.</returns>
    public static abstract TSelf Create(
        ulong chainId, ulong nonce, ITxInput input, TTxParams txParams, TTxGasParams gasParams
    );

    /// <summary>
    /// Returns a copy of this transaction with updated gas and fee parameters.
    /// </summary>
    /// <param name="gasParams">Gas and fee parameters to apply.</param>
    /// <returns>A new transaction instance with updated gas parameters.</returns>
    public TSelf WithGasParams(TTxGasParams gasParams);

    /// <summary>
    /// Returns a copy of this transaction with an updated sender nonce.
    /// </summary>
    /// <param name="nonce">Sender nonce to apply.</param>
    /// <returns>A new transaction instance with the updated nonce.</returns>
    public TSelf WithNonce(ulong nonce);

    /// <summary>
    /// Returns a copy of this transaction with gas fee values scaled by <paramref name="multiplier"/> / <paramref name="divider"/>.
    /// </summary>
    /// <param name="multiplier">Numerator for the scaling factor.</param>
    /// <param name="divider">Denominator for the scaling factor.</param>
    /// <param name="minimumIncrement">Minimum absolute increment to enforce.</param>
    /// <returns>A new transaction instance with adjusted gas fee values.</returns>
    public TSelf IncrementGasFeeByFactor(UInt256 multiplier, UInt256 divider, UInt256 minimumIncrement);
}
