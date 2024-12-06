using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Parameters;
public class AbiInputParameter
{
    [JsonRequired]
    public string Name { get; set; } = null!;
    [JsonRequired]
    public string Type { get; set; } = null!;

    public string? InternalType { get; set; }
    public AbiInputParameter[]? Components { get; set; }

    public string GetFunctionSignatureTypeString()
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

            sb.Append(component.GetFunctionSignatureTypeString());

            if(!isLastComponent)
            {
                sb.Append(',');
            }
        }

        sb.Append(')');

        return Type.Replace("tuple", sb.ToString());
    }
}
