using EtherSharp.Tx;
using EtherSharp.Tx.Types;

namespace EtherSharp.Client.Services.GasFeeProvider;

/// <summary>
/// Provides chain-specific gas parameter estimation for a transaction input.
/// </summary>
/// <typeparam name="TTxParams">The transaction parameter type used by the selected transaction format.</typeparam>
/// <typeparam name="TTxGasParams">The gas parameter type produced by the estimator.</typeparam>
public interface IGasFeeProvider<TTxParams, TTxGasParams>
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams
{
    /// <summary>
    /// Estimates gas-related parameters required to submit the provided transaction.
    /// </summary>
    /// <param name="txInput">The transaction payload to estimate gas usage for.</param>
    /// <param name="txParams">The transaction parameters used as estimation context.</param>
    /// <param name="cancellationToken">A token used to cancel the estimation request.</param>
    /// <returns>The estimated gas parameter object for the transaction type.</returns>
    public Task<TTxGasParams> EstimateGasParamsAsync(
        ITxInput txInput,
        TTxParams txParams,
        CancellationToken cancellationToken
    );
}
