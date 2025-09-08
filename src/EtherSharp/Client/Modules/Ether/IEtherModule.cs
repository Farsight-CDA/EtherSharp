using EtherSharp.Contract;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Modules.Ether;

public interface IEtherModule
{
    public Task<BigInteger> GetBalanceAsync(Address address, TargetBlockNumber blockNumber = default);
    public Task<BigInteger> GetBalanceAsync(IEVMContract contract, TargetBlockNumber blockNumber = default);
}
