using EtherSharp.Tx;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
public interface IEtherTxApi : IEtherApi
{
    public TxInput Transfer(string receiver, BigInteger amount);
}
