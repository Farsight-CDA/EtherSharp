using EtherSharp.Contract;
using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.EtherModule;
public interface IEtherTxModule : IEtherModule
{
    public Task<BigInteger> GetMyBalanceAsync(TargetBlockNumber blockNumber = default);

    public ITxInput Transfer(Address receiver, BigInteger amount);
    public ITxInput Transfer(IPayableContract contract, BigInteger amount);
}
