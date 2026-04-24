using EtherSharp.Contract;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Services.ContractFactory;

internal sealed class ContractFactory(IEtherClient etherClient)
{
    public TContract Create<TContract>(in Address address)
        where TContract : IEVMContract
    {
        RuntimeHelpers.RunModuleConstructor(typeof(TContract).Module.ModuleHandle);

        return GeneratedContractRegistry.TryCreate<TContract>(etherClient, address, out var contract)
            ? contract
            : throw new NotSupportedException($"Could not find a generated contract factory for contract interface {typeof(TContract)}");
    }
}
