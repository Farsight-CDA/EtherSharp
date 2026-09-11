namespace EtherSharp.Interpreter.Runtime;

/// <summary>Owns an invocation's remaining execution gas independently of its bytecode frame.</summary>
/// <param name="limit">The execution gas allocated to the invocation.</param>
public sealed class GasBudget(ulong limit)
{
    /// <summary>Gets the remaining execution gas.</summary>
    public ulong Remaining { get; private set; } = limit;

    /// <summary>Charges gas without modifying the budget when the charge cannot be paid.</summary>
    public bool TryCharge(ulong amount)
    {
        if(amount > Remaining)
        {
            return false;
        }

        Remaining -= amount;
        return true;
    }

    /// <summary>Consumes the entire remaining budget on an exceptional halt.</summary>
    public void ConsumeAll()
        => Remaining = 0;

    /// <summary>Transfers an allocation out of this budget into an independent child budget.</summary>
    public GasBudget Forward(ulong amount)
        => !TryCharge(amount)
            ? throw new ArgumentOutOfRangeException(nameof(amount), "Cannot forward more gas than remains.")
            : new GasBudget(amount);

    /// <summary>Returns unused child gas. This is not an EVM storage refund.</summary>
    public void Return(ulong amount)
        => Remaining = checked(Remaining + amount);
}
