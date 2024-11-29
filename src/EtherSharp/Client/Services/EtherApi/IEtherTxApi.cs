using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
public interface IEtherTxApi : IEtherApi
{
    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber blockNumber = default);

    public TxInput Transfer(string receiver, BigInteger amount);
    public TxInput Transfer(Address receiver, BigInteger amount);
}
