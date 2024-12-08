﻿using EtherSharp.Contract;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Services.ContractFactory;
internal class ContractFactory(IEtherClient etherClient) : IContractFactory
{
    private readonly Lock _lock = new Lock();
    private readonly IEtherClient _etherClient = etherClient;
    private readonly Dictionary<Type, Func<string, IEVMContract>> _factoryDelegates = [];

    public TContract Create<TContract>(string contractAddress)
    {
        if(!_factoryDelegates.TryGetValue(typeof(TContract), out var factoryDelegate))
        {
            factoryDelegate = GetContractFactoryDelegate(typeof(TContract));
            lock(_lock)
            {
                _factoryDelegates.TryAdd(typeof(TContract), factoryDelegate);
            }
        }

        return (TContract) factoryDelegate(contractAddress);
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

    private Func<string, IEVMContract> GetContractFactoryDelegate(Type contractInterfaceType)
    {
        var assembly = contractInterfaceType.Assembly;
        var contractType = assembly.GetTypes()
            .Where(x => x.Name == $"{contractInterfaceType.Name}_Generated_Implementation")
            .SingleOrDefault() ?? throw new NotSupportedException($"Could not find implementation for contract interface {contractInterfaceType}");

        if(RuntimeFeature.IsDynamicCodeCompiled)
        {
            var ctor = contractType.GetConstructor([typeof(IEtherClient), typeof(string)])
                ?? throw new NotSupportedException("Constructor not found.");

            var contractAddressParam = Expression.Parameter(typeof(string), "contractAddress");

            var newExpr = Expression.New(
                ctor,
                Expression.Constant(_etherClient),
                contractAddressParam
            );

            return Expression.Lambda<Func<string, IEVMContract>>(newExpr, contractAddressParam).Compile();
        }
        else
        {
            return contractAddress => (IEVMContract) (Activator.CreateInstance(contractType, _etherClient, contractAddress)
                ?? throw new NotSupportedException());
        }
    }
}
