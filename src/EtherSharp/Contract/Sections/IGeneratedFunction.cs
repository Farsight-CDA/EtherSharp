namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for a generated contract function type.
/// </summary>
public interface IGeneratedFunction
{
    /// <summary>
    /// Gets the function selector bytes.
    /// </summary>
    /// <returns></returns>
    public abstract static ReadOnlyMemory<byte> SelectorBytes { get; }
}
