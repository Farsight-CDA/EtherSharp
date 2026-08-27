using EtherSharp.Contract;
using System.Runtime.CompilerServices;

namespace EtherSharp.Types;

/// <summary>
/// Builds immutable <see cref="EventFilter"/> values from emitting-address and topic conditions.
/// </summary>
/// <remarks>
/// Reconfiguring addresses or a topic slot replaces that complete condition.
/// </remarks>
public sealed class EventFilterBuilder
{
    private ReadOnlyMemory<Address> _contractAddresses;
    private ReadOnlyMemory<Bytes32> _topic0;
    private ReadOnlyMemory<Bytes32> _topic1;
    private ReadOnlyMemory<Bytes32> _topic2;
    private ReadOnlyMemory<Bytes32> _topic3;

    /// <summary>
    /// Gets whether no address or topic conditions have been configured.
    /// </summary>
    public bool IsEmpty
        => _contractAddresses.IsEmpty
            && _topic0.IsEmpty
            && _topic1.IsEmpty
            && _topic2.IsEmpty
            && _topic3.IsEmpty;

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contracts whose addresses will be matched.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithContracts(params ReadOnlySpan<IEVMContract> contracts)
    {
        if(contracts.IsEmpty)
        {
            throw new ArgumentException("An address filter must contain at least one address.", nameof(contracts));
        }

        var addresses = new Address[contracts.Length];
        for(int i = 0; i < addresses.Length; i++)
        {
            addresses[i] = contracts[i].Address;
        }

        _contractAddresses = FilterNormalization.Normalize(addresses);
        return this;
    }

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithContractAddresses(params ReadOnlySpan<Address> contractAddresses)
    {
        if(contractAddresses.IsEmpty)
        {
            throw new ArgumentException("An address filter must contain at least one address.", nameof(contractAddresses));
        }

        _contractAddresses = FilterNormalization.Normalize(contractAddresses.ToArray());
        return this;
    }

    /// <summary>
    /// Restricts logs to any of the provided emitting contracts.
    /// </summary>
    /// <param name="contracts">Contracts whose addresses will be matched.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithContracts(params IEnumerable<IEVMContract> contracts)
    {
        Address[] addresses = [.. contracts.Select(static contract => contract.Address)];
        if(addresses.Length == 0)
        {
            throw new ArgumentException("An address filter must contain at least one address.", nameof(contracts));
        }

        _contractAddresses = FilterNormalization.Normalize(addresses);
        return this;
    }

    /// <summary>
    /// Restricts logs to any of the provided emitting contract addresses.
    /// </summary>
    /// <param name="contractAddresses">Contract addresses to match.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithContractAddresses(params IEnumerable<Address> contractAddresses)
    {
        Address[] addresses = [.. contractAddresses];
        if(addresses.Length == 0)
        {
            throw new ArgumentException("An address filter must contain at least one address.", nameof(contractAddresses));
        }

        _contractAddresses = FilterNormalization.Normalize(addresses);
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 0 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithTopic0(params ReadOnlySpan<Bytes32> topics)
    {
        if(topics.IsEmpty)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic0 = FilterNormalization.Normalize(topics.ToArray());
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 0 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithTopic0(params IEnumerable<Bytes32> topics)
    {
        Bytes32[] values = [.. topics];
        if(values.Length == 0)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic0 = FilterNormalization.Normalize(values);
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 1 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithTopic1(params ReadOnlySpan<Bytes32> topics)
    {
        if(topics.IsEmpty)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic1 = FilterNormalization.Normalize(topics.ToArray());
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 1 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithTopic1(params IEnumerable<Bytes32> topics)
    {
        Bytes32[] values = [.. topics];
        if(values.Length == 0)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic1 = FilterNormalization.Normalize(values);
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 2 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithTopic2(params ReadOnlySpan<Bytes32> topics)
    {
        if(topics.IsEmpty)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic2 = FilterNormalization.Normalize(topics.ToArray());
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 2 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithTopic2(params IEnumerable<Bytes32> topics)
    {
        Bytes32[] values = [.. topics];
        if(values.Length == 0)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic2 = FilterNormalization.Normalize(values);
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 3 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    [OverloadResolutionPriority(1)]
    public EventFilterBuilder WithTopic3(params ReadOnlySpan<Bytes32> topics)
    {
        if(topics.IsEmpty)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic3 = FilterNormalization.Normalize(topics.ToArray());
        return this;
    }

    /// <summary>
    /// Matches logs where topic slot 3 equals any provided value.
    /// </summary>
    /// <param name="topics">Allowed topic values.</param>
    /// <returns>This builder.</returns>
    public EventFilterBuilder WithTopic3(params IEnumerable<Bytes32> topics)
    {
        Bytes32[] values = [.. topics];
        if(values.Length == 0)
        {
            throw new ArgumentException("A configured topic slot must contain at least one value.", nameof(topics));
        }

        _topic3 = FilterNormalization.Normalize(values);
        return this;
    }

    /// <summary>
    /// Creates an immutable snapshot of the configured conditions.
    /// </summary>
    /// <returns>The configured event filter.</returns>
    public EventFilter Build()
        => EventFilter.FromOwned(_contractAddresses, EventTopics.FromOwned(_topic0, _topic1, _topic2, _topic3));

}
