namespace EtherSharp.Contract;

/// <summary>
/// Marks an interface as a contract interface with a given ABI.
/// </summary>
/// <param name="file">The ABI file name</param>
[AttributeUsage(AttributeTargets.Interface)]
public class AbiFileAttribute(string file) : Attribute
{
    /// <summary>
    /// The Abi file path.
    /// </summary>
    public string File { get; } = file;
}
