using EtherSharp.Numerics;

namespace EtherSharp.Types;

/// <summary>
/// Describes a single account entry in an <c>eth_call</c> state override set.
/// </summary>
public sealed record AccountOverride
{
    /// <summary>
    /// Gets the account balance override.
    /// </summary>
    public UInt256? Balance { get; }

    /// <summary>
    /// Gets the account nonce override.
    /// </summary>
    public ulong? Nonce { get; }

    /// <summary>
    /// Gets the account code override.
    /// </summary>
    public ReadOnlyMemory<byte>? Code { get; }

    /// <summary>
    /// Gets the replacement storage view.
    /// </summary>
    public IReadOnlyDictionary<Bytes32, Bytes32>? State { get; }

    /// <summary>
    /// Gets the storage patch to apply to the existing view.
    /// </summary>
    public IReadOnlyDictionary<Bytes32, Bytes32>? StateDiff { get; }

    /// <summary>
    /// Initializes an account state override.
    /// </summary>
    /// <exception cref="ArgumentException"><paramref name="state"/> and <paramref name="stateDiff"/> are both specified.</exception>
    public AccountOverride(
        UInt256? balance = null,
        ulong? nonce = null,
        ReadOnlyMemory<byte>? code = null,
        IReadOnlyDictionary<Bytes32, Bytes32>? state = null,
        IReadOnlyDictionary<Bytes32, Bytes32>? stateDiff = null)
    {
        if(state is not null && stateDiff is not null)
        {
            throw new ArgumentException("State and StateDiff cannot both be specified.", nameof(stateDiff));
        }

        Balance = balance;
        Nonce = nonce;
        Code = code;
        State = state;
        StateDiff = stateDiff;
    }
}
