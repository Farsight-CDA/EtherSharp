using EtherSharp.Client;
using EtherSharp.Types;

namespace EtherSharp.Contract;

/// <summary>
/// Marker interface for source-generated contract bindings and their shared runtime contract surface.
/// Contract interfaces inheriting from <see cref="IEVMContract"/> are discovered by the EtherSharp generator,
/// which emits concrete implementations that use these members for calls against the target contract address.
/// </summary>
public interface IEVMContract
{
    /// <summary>
    /// Gets the on-chain address of the contract instance.
    /// </summary>
    public Address Address { get; }

    /// <summary>
    /// Gets the client used to execute RPC calls for this contract.
    /// </summary>
    /// <returns>The client used by this contract instance.</returns>
    protected IEtherClient GetClient();
}
