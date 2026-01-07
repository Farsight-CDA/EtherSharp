using EtherSharp.Numerics;
using EtherSharp.Tx;
using EtherSharp.Tx.Types;
using EtherSharp.Types;

namespace EtherSharp.Client.Services.ResiliencyLayer;

public record TxSubmissionStorage(
    ulong ChainId,
    uint Sequence,
    uint Nonce,
    string TxHash,
    string SignedTx,
    string To,
    UInt256 Value,
    byte[] CallData,
    byte[] TxParams,
    byte[] TxGasParams
)
{
    public TxSubmission<TTxParams, TTxGasParams> ToTxSubmission<TTxParams, TTxGasParams>()
        where TTxParams : class, ITxParams<TTxParams>
        where TTxGasParams : class, ITxGasParams<TTxGasParams>
        => new TxSubmission<TTxParams, TTxGasParams>(
            ChainId,
            Sequence,
            TxHash,
            SignedTx,
            new TxInput(Address.FromString(To), Value, CallData),
            TTxParams.Decode(TxParams),
            TTxGasParams.Decode(TxGasParams)
        );
}
