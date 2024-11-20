namespace EtherSharp.Contract;

[AttributeUsage(AttributeTargets.Interface)]
#pragma warning disable CS9113
public class AbiFileAttribute(string file) : Attribute
#pragma warning restore CS9113
{
}
