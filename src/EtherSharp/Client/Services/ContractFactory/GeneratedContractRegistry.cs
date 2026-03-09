using EtherSharp.Contract;
using EtherSharp.Types;
using System.Collections.Frozen;

namespace EtherSharp.Client.Services.ContractFactory;

/// <summary>
/// Stores generated contract registrations emitted by the EtherSharp source generator.
/// </summary>
public static class GeneratedContractRegistry
{
    private static readonly Lock _lock = new Lock();
    private static readonly Dictionary<Type, Func<IEtherClient, Address, IEVMContract>> _registrations = [];

    /// <summary>
    /// Registers a generated contract factory.
    /// </summary>
    /// <typeparam name="TContract">The contract interface type.</typeparam>
    /// <param name="factory">Factory delegate used to instantiate the generated implementation.</param>
    public static void Register<TContract>(Func<IEtherClient, Address, TContract> factory)
        where TContract : class, IEVMContract
    {
        ArgumentNullException.ThrowIfNull(factory);

        lock(_lock)
        {
            _registrations[typeof(TContract)] = (etherClient, address) => factory(etherClient, address);
        }
    }

    internal static FrozenDictionary<Type, Func<Address, IEVMContract>> CloneFor(IEtherClient etherClient)
    {
        lock(_lock)
        {
            return _registrations.ToFrozenDictionary(
                x => x.Key,
                x => new Func<Address, IEVMContract>(address => x.Value(etherClient, address))
            );
        }
    }
}
