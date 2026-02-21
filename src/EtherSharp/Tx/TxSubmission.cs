using EtherSharp.Client.Services.ResiliencyLayer;
using EtherSharp.Tx.Types;

namespace EtherSharp.Tx;

/// <summary>
/// Captures one signed submission attempt for a specific nonce.
/// </summary>
/// <typeparam name="TTxParams">Transaction parameter type.</typeparam>
/// <typeparam name="TTxGasParams">Gas parameter type.</typeparam>
/// <param name="ChainId">Chain id used when signing this submission.</param>
/// <param name="Sequence">Submission revision for the same nonce; starts at <c>0</c> and increments for replacements.</param>
/// <param name="TxHash">Transaction hash of this signed payload.</param>
/// <param name="SignedTx">Raw signed transaction payload used for publish.</param>
/// <param name="Call">Original call input this submission was built from.</param>
/// <param name="Params">Transaction parameters used for encoding/signing.</param>
/// <param name="GasParams">Gas parameters used for encoding/signing.</param>
public record TxSubmission<TTxParams, TTxGasParams>(
    ulong ChainId,
    uint Sequence,
    string TxHash,
    string SignedTx,
    ITxInput Call,
    TTxParams Params,
    TTxGasParams GasParams
)
    where TTxParams : class, ITxParams<TTxParams>
    where TTxGasParams : class, ITxGasParams<TTxGasParams>
{
    /// <summary>
    /// Converts this submission to its persisted resiliency representation for a nonce.
    /// </summary>
    /// <param name="nonce">Account nonce associated with this submission chain.</param>
    /// <returns>The storage model used by the resiliency layer.</returns>
    public TxSubmissionStorage ToStorageType(uint nonce)
        => new TxSubmissionStorage(
            ChainId,
            Sequence,
            nonce,
            TxHash,
            SignedTx,
            Call.To?.String,
            Call.Value,
            Call.Data.ToArray(),
            Params.Encode(),
            GasParams.Encode()
        );
}
