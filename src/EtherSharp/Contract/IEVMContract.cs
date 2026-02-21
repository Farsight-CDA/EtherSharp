using EtherSharp.Client;
using EtherSharp.Types;

namespace EtherSharp.Contract;

public interface IEVMContract
{
    public Address Address { get; }

    protected IEtherClient GetClient();
}
