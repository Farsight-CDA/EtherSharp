using System.Reflection;

namespace EtherSharp.Client.Services.ContractFactory;

/// <summary>
/// Discovers and registers contract types so they can be materialized through client APIs.
/// </summary>
public interface IContractFactory
{
    /// <summary>
    /// Scans an assembly for supported contract types and adds them to the factory registry.
    /// </summary>
    /// <param name="assembly">The assembly that contains contract types to register.</param>
    public void AddContractTypesFromAssembly(Assembly assembly);
}
