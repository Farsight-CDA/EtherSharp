using Epoche;
using EtherSharp.Generator.Abi.Parameters;
using System.Text;
using System.Text.Json.Serialization;

namespace EtherSharp.Generator.Abi.Members;
public class FunctionAbiMember : AbiMember
{
    [JsonRequired]
    public string Name { get; set; } = null!;

    [JsonRequired]
    public StateMutability StateMutability { get; set; }

    [JsonRequired]
    public AbiInputParameter[] Inputs { get; set; } = null!;

    [JsonRequired]
    public AbiOutputParameter[] Outputs { get; set; } = null!;

    public byte[] GetSignatureBytes()
    {
        var sb = new StringBuilder();
        sb.Append(Name);
        sb.Append('(');

        for(int i = 0; i < Inputs.Length; i++)
        {
            var input = Inputs[i];
            bool isLastInput = i == Inputs.Length - 1;

            sb.Append(input.Type);

            if(!isLastInput)
            {
                sb.Append(",");
            }
        }

        sb.Append(")");

        string signature = sb.ToString();
        byte[] hash = Keccak256.ComputeHash(signature);

        byte[] selector = new byte[4];
        hash.AsSpan().Slice(0, 4).CopyTo(selector);
        return selector;
    }
}
