using EtherSharp.Client.Services.RPC;

namespace EtherSharp.Client;
public interface IInternalEtherClient
{
    public IServiceProvider Provider { get; }
    public IRpcClient RPC { get; }
}
