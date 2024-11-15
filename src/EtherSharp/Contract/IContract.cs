using EtherSharp.Tx;

namespace EtherSharp.Contract;
public interface IContract
{
    public string Address { get; }

    public TxInput<string> Relay(string receiver, string payload);
}

