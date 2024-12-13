using EtherSharp.Contract;
using EtherSharp.Types;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Services.ContractFactory;
internal class ContractFactory(IEtherClient etherClient) : IContractFactory
{
    private readonly Lock _lock = new Lock();
    private readonly IEtherClient _etherClient = etherClient;
    private readonly Dictionary<Type, Func<Address, IEVMContract>> _factoryDelegates = [];

    public TContract Create<TContract>(Address address)
    {
        if(!_factoryDelegates.TryGetValue(typeof(TContract), out var factoryDelegate))
        {
            factoryDelegate = GetContractFactoryDelegate(typeof(TContract));
            lock(_lock)
            {
                _factoryDelegates.TryAdd(typeof(TContract), factoryDelegate);
            }
        }

        return (TContract) factoryDelegate(address);
    }

    public void AddContractTypesFromAssembly(Assembly assembly)
    {
        var factoryDelegates = assembly.GetTypes()
            .Where(x => x.GetInterface(nameof(IEVMContract)) is not null)
            .Where(x => x.IsInterface)
            .Select(x => (x, GetContractFactoryDelegate(x)));

        lock(_lock)
        {
            foreach(var (interfaceType, factoryDelegate) in factoryDelegates)
            {
                _factoryDelegates.TryAdd(interfaceType, factoryDelegate);
            }
        }
    }

    private Func<Address, IEVMContract> GetContractFactoryDelegate(Type contractInterfaceType)
    {
        var assembly = contractInterfaceType.Assembly;
        var contractType = assembly.GetTypes()
            .Where(x => x.Name == $"{contractInterfaceType.Name}_Generated_Implementation")
            .SingleOrDefault() ?? throw new NotSupportedException($"Could not find implementation for contract interface {contractInterfaceType}");

        if(RuntimeFeature.IsDynamicCodeCompiled)
        {
            var ctor = contractType.GetConstructor([typeof(IEtherClient), typeof(Address)])
                ?? throw new NotSupportedException("Constructor not found.");

            var contractAddressParam = Expression.Parameter(typeof(Address), "address");

            var newExpr = Expression.New(
                ctor,
                Expression.Constant(_etherClient),
                contractAddressParam
            );

            return Expression.Lambda<Func<Address, IEVMContract>>(newExpr, contractAddressParam).Compile();
        }
        else
        {
            return contractAddress => (IEVMContract) (Activator.CreateInstance(contractType, _etherClient, contractAddress)
                ?? throw new NotSupportedException());
        }
    }
}
