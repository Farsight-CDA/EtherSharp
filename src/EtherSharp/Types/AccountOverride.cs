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

    /// <inheritdoc />
    public bool Equals(AccountOverride? other)
        => ReferenceEquals(this, other)
            || (other is not null
                && Balance == other.Balance
                && Nonce == other.Nonce
                && CodeEquals(Code, other.Code)
                && StateEquals(State, other.State)
                && StateEquals(StateDiff, other.StateDiff));

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Balance);
        hashCode.Add(Nonce);
        hashCode.Add(Code is not null);
        if(Code is { } code)
        {
            foreach(byte value in code.Span)
            {
                hashCode.Add(value);
            }
        }

        hashCode.Add(State is not null);
        hashCode.Add(GetStateHashCode(State));
        hashCode.Add(StateDiff is not null);
        hashCode.Add(GetStateHashCode(StateDiff));
        return hashCode.ToHashCode();
    }

    private static bool CodeEquals(ReadOnlyMemory<byte>? left, ReadOnlyMemory<byte>? right)
        => left is null
            ? right is null
            : right is { } rightCode && left.Value.Span.SequenceEqual(rightCode.Span);

    private static bool StateEquals(
        IReadOnlyDictionary<Bytes32, Bytes32>? left,
        IReadOnlyDictionary<Bytes32, Bytes32>? right)
    {
        if(left is null || right is null)
        {
            return left is null && right is null;
        }

        if(left.Count != right.Count)
        {
            return false;
        }

        foreach(var (slot, value) in left)
        {
            if(!right.TryGetValue(slot, out var otherValue) || value != otherValue)
            {
                return false;
            }
        }

        return true;
    }

    private static int GetStateHashCode(IReadOnlyDictionary<Bytes32, Bytes32>? state)
    {
        if(state is null)
        {
            return 0;
        }

        int entriesHashCode = 0;
        foreach(var (slot, value) in state)
        {
            entriesHashCode ^= HashCode.Combine(slot, value);
        }

        return HashCode.Combine(state.Count, entriesHashCode);
    }
}
