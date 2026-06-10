using EtherSharp.Types;

namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for the errors section of a generated contract schema.
/// </summary>
public interface IErrorsSection
{
    /// <summary>
    /// Gets all error selectors defined in the contract.
    /// </summary>
    /// <returns></returns>
    public abstract static Bytes4[] GetSelectors();
}
