using EtherSharp.Types;

namespace EtherSharp.Contract.Sections;

/// <summary>
/// Interface for a generated contract error type.
/// </summary>
public interface IGeneratedError
{
    /// <summary>
    /// Gets the error signature bytes.
    /// </summary>
    /// <returns></returns>
    public abstract static Bytes4 Signature { get; }
}
