using EtherSharp.Tx;
using EtherSharp.Types;
using System.Numerics;

namespace EtherSharp.Client.Services.EtherApi;
public interface IEtherTxApi : IEtherApi
{
    public TxInput Transfer(string receiver, BigInteger amount);
    public TxInput Transfer(Address receiver, BigInteger amount);
}
