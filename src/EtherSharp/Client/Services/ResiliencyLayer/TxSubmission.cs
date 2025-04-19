using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.ResiliencyLayer;
public record TxSubmission(
    string TxHash, 
    Address To,
    BigInteger Value,
    byte[] Calldata,
    byte[] TxParams,
    byte[] TxGasParams
);