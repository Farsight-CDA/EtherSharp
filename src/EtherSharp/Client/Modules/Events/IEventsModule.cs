using EtherSharp.Contract;
using EtherSharp.Realtime.Events;
using EtherSharp.Types;
using System.Runtime.CompilerServices;

namespace EtherSharp.Client.Modules.Events;

/// <summary>
/// Fluent builder for configuring event topic/address filters before execution.
/// </summary>
/// <remarks>
/// Reconfiguring addresses or a topic slot replaces that complete condition.
/// </remarks>
/// <typeparam name="TLog">Typed log representation decoded from chain logs.</typeparam>
public interface IEventsModule<TLog> : IConfiguredEventsModule<TLog>
    where TLog : ITxLog<TLog>
{
    /// <summary>
    /// Matches logs where topic slot 0 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithTopic0(params ReadOnlySpan<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 0 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithTopic0(params IEnumerable<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 1 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithTopic1(params ReadOnlySpan<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 1 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithTopic1(params IEnumerable<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 2 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithTopic2(params ReadOnlySpan<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 2 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithTopic2(params IEnumerable<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 3 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithTopic3(params ReadOnlySpan<Bytes32> topics);

    /// <summary>
    /// Matches logs where topic slot 3 equals any of the provided values.
    /// </summary>
    /// <param name="topics">Allowed topic values (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithTopic3(params IEnumerable<Bytes32> topics);

    /// <summary>
    /// Applies a complete immutable event filter to this module.
    /// </summary>
    /// <param name="eventFilter">The address and topic conditions to apply.</param>
    /// <returns>The configured module.</returns>
    /// <remarks>Replaces any address or topic conditions previously configured on this module.</remarks>
    /// <exception cref="InvalidOperationException">A complete event filter is already applied.</exception>
    public IConfiguredEventsModule<TLog> WithEventFilter(in EventFilter eventFilter);

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithContractAddresses(params ReadOnlySpan<Address> contractAddresses);

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match (OR semantics).</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithContractAddresses(params IEnumerable<Address> contractAddresses);

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contract instances whose addresses will be matched.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    [OverloadResolutionPriority(1)]
    public IEventsModule<TLog> WithContracts(params ReadOnlySpan<IEVMContract> contracts);

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contract instances whose addresses will be matched.</param>
    /// <returns>The same module instance for fluent chaining.</returns>
    public IEventsModule<TLog> WithContracts(params IEnumerable<IEVMContract> contracts);
}
