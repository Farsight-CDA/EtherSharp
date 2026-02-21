using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.ResiliencyLayer;

/// <summary>
/// Persisted snapshot of a transaction submission attempt used by the resiliency layer
/// to recover pending nonce flows across process restarts.
/// </summary>
public record TxSubmissionStorage(
    ulong ChainId,
    uint Sequence,
    uint Nonce,
    string TxHash,
    string SignedTx,
    string? To,
    UInt256 Value,
    byte[] CallData,
    byte[] TxParams,
    byte[] TxGasParams
)
{
    internal TxSubmission<TTxParams, TTxGasParams> ToTxSubmission<TTxParams, TTxGasParams>()
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
        => new TxSubmission<TTxParams, TTxGasParams>(
            ChainId,
            Sequence,
            TxHash,
            SignedTx,
            To is null
                ? new ContractDeployment(new EVMByteCode(CallData), Value)
                : new TxInput(Address.FromString(To), Value, CallData),
            TTxParams.Decode(TxParams),
            TTxGasParams.Decode(TxGasParams)
        );
}
