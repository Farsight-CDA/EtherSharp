using EtherSharp.Client;

namespace EtherSharp.Contract;
public interface IEVMContract
{
    public string ContractAddress { get; }

    protected IEtherClient GetClient();
}