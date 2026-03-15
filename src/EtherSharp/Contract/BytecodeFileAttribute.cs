namespace EtherSharp.Contract;

/// <summary>
/// Marks an interface as a contract interface with a creation bytecode.
/// </summary>
/// <param name="file">The bytecode file name</param>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class BytecodeFileAttribute(string file) : Attribute
{
    /// <summary>
    /// The Abi file path.
    /// </summary>
    public string File { get; } = file;
}
