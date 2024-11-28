using EtherSharp.Tx;
using System.Numerics;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public Task<string> SendAsync<T>(ITxInput call, ulong gas, BigInteger maxFeePerGas, BigInteger maxPriorityFeePerGas);
}
