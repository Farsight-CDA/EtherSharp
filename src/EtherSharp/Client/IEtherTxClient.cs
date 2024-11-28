using EtherSharp.Tx;

namespace EtherSharp.Client;
public interface IEtherTxClient : IEtherClient
{
    public Task<string> SendAsync<T>(TxInput<T> call);
    public Task<string> SendAsync(TxInput call);
}
