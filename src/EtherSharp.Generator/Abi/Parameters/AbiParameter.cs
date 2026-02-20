using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;

public class AbiParameter
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;

    public string? InternalType { get; set; }
    public AbiParameter[]? Components { get; set; }

    public AbiParameter(string name, string type, string? internalType, AbiParameter[]? components)
    {
        Name = name;
        Type = type;
        InternalType = internalType;
        Components = components;
    }

    public string GetSignatureTypeString()
    {
        if(!Type.Contains("tuple"))
        {
            return Type;
        }

        var sb = new StringBuilder();
        sb.Append('(');

        for(int i = 0; i < Components?.Length; i++)
        {
            bool isLastComponent = i == Components.Length - 1;
            var component = Components[i];

            sb.Append(component.GetSignatureTypeString());

            if(!isLastComponent)
            {
                sb.Append(',');
            }
        }

        sb.Append(')');

        return Type.Replace("tuple", sb.ToString());
    }
}
