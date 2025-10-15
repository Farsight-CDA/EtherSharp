using Epoche;
using EtherSharp.Generator.Abi.Parameters;
using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;

public class EventAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    [JsonRequired]
    [JsonPropertyName("anonymous")]
    public bool IsAnonymous { get; set; }

    [JsonRequired]
    public AbiEventParameter[] Inputs { get; set; } = null!;

    public byte[] GetEventTopic(out string eventSignature)
    {
        var sb = new StringBuilder();
        sb.Append(Name);
        sb.Append('(');

        for(int i = 0; i < Inputs.Length; i++)
        {
            var input = Inputs[i];
            bool isLastInput = i == Inputs.Length - 1;

            sb.Append(input.GetSignatureTypeString());

            if(!isLastInput)
            {
                sb.Append(',');
            }
        }

        sb.Append(')');

        eventSignature = sb.ToString();
        return Keccak256.ComputeHash(eventSignature);
    }
}
