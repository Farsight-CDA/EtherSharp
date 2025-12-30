namespace EtherSharp.Contract;

/// <summary>
/// Marks an interface as a contract interface with a given ABI.
/// </summary>
/// <param name="file">The ABI file name</param>
[AttributeUsage(AttributeTargets.Interface)]
#pragma warning disable CS9113
public class AbiFileAttribute(string file) : Attribute
#pragma warning restore CS9113
{
}
