using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Modules.Events;

/// <summary>
/// Fluent builder for configuring event topic/address filters before execution.
/// </summary>
/// <remarks>
/// Each topic index and contract-address filter can only be configured once per module instance.
/// </remarks>
/// <typeparam name="TLog">Typed log representation decoded from chain logs.</typeparam>
public interface IEventsModule<TLog> : IConfiguredEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    /// <summary>
    /// Matches logs where a specific indexed topic equals the provided value.
    /// </summary>
    /// <param name="topic">Topic value to match (hex string).</param>
    /// <param name="index">Topic slot index (0-based).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasTopic(string topic, int index = 0);

    /// <summary>
    /// Matches logs where a specific indexed topic equals any of the provided values.
    /// </summary>
    /// <param name="index">Topic slot index (0-based).</param>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasTopics(int index = 0, params ReadOnlySpan<string> topics);

    /// <summary>
    /// Matches logs where a specific indexed topic equals any of the provided values.
    /// </summary>
    /// <param name="index">Topic slot index (0-based).</param>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasTopics(int index = 0, params IEnumerable<string> topics);

    /// <summary>
    /// Restricts logs to a single emitting contract address.
    /// </summary>
    /// <param name="contractAddress">Contract address to match.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasContractAddress(Address contractAddress);

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasContractAddresses(params ReadOnlySpan<Address> contractAddresses);

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasContractAddresses(params IEnumerable<Address> contractAddresses);

    /// <summary>
    /// Restricts logs to a single emitting contract.
    /// </summary>
    /// <param name="contract">Contract instance whose address will be matched.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasContract(IEVMContract contract);

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contract instances whose addresses will be matched.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> HasContracts(params ReadOnlySpan<IEVMContract> contracts);

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contract instances whose addresses will be matched.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> HasContracts(params IEnumerable<IEVMContract> contracts);
}
