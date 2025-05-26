using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
public interface IEtherTxApi : IEtherApi
{
    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber blockNumber = default);

    public ITxInput Transfer(Address receiver, BigInteger amount);
    public ITxInput Transfer(IPayableContract contract, BigInteger amount);
}
