using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;

public class AbiParameter(string name, string type, string? internalType, AbiParameter[]? components)
{
    [JsonRequired]
    public string Name { get; set; } = name;
    [JsonRequired]
    public string Type { get; set; } = type;

    public string? InternalType { get; set; } = internalType;
    public AbiParameter[]? Components { get; set; } = components;

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
