using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.ResiliencyLayer;
public record TxSubmissionStorage(
    string TxHash,
    string SignedTx,
    Address To,
    BigInteger Value,
    byte[] CallData,
    byte[] TxParams,
    byte[] TxGasParams
);
