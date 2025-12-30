namespace EtherSharp.Contract;

/// <summary>
/// Marks an interface as a contract interface with a creation bytecode.
/// </summary>
/// <param name="file">The bytecode file name</param>
[AttributeUsage(AttributeTargets.Interface)]
#pragma warning disable CS9113
public class BytecodeFileAttribute(string file) : Attribute
#pragma warning restore CS9113
{
}
