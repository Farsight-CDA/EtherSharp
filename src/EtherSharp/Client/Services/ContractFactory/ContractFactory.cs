using EtherSharp.Contract;
using EtherSharp.Types;
using System.Collections.Frozen;

namespace EtherSharp.Client.Services.ContractFactory;

internal sealed class ContractFactory(IEtherClient etherClient)
{
    private readonly FrozenDictionary<Type, Func<Address, IEVMContract>> _factoryDelegates = GeneratedContractRegistry.CloneFor(etherClient);

    public TContract Create<TContract>(in Address address)
        => (TContract) (_factoryDelegates.TryGetValue(typeof(TContract), out var factoryDelegate)
            ? factoryDelegate(address)
            : throw new NotSupportedException($"Could not find a generated contract factory for contract interface {typeof(TContract)}"));
}
