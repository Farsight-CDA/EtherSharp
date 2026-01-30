namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for the functions section of a generated contract schema.
/// </summary>
public interface IFunctionsSection
{
    /// <summary>
    /// Gets all function selectors defined in the contract.
    /// </summary>
    /// <returns></returns>
    public abstract static ReadOnlyMemory<byte>[] GetSelectors();
}

