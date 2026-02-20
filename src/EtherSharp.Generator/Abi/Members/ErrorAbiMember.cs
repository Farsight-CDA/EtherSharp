using Epoche;
using EtherSharp.Generator.Abi.Parameters;
using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;

public class ErrorAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    [JsonRequired]
    public AbiParameter[] Inputs { get; set; } = null!;

    public byte[] GetSignatureBytes(out string errorSignature)
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

        errorSignature = sb.ToString();
        byte[] hash = Keccak256.ComputeHash(errorSignature);
        return hash.AsSpan().Slice(0, 4).ToArray();
    }
}
