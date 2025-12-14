using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SourceWriters.Components;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters;

public class ContractErrorSectionWriter(ErrorTypeWriter errorTypeWriter)
{
    private readonly ErrorTypeWriter _errorTypeWriter = errorTypeWriter;

    public void GenerateContractErrorSection(InterfaceBuilder interfaceBuilder, ClassBuilder implementationBuilder, IEnumerable<ErrorAbiMember> errorMembers)
    {
        var sectionBuilder = new ClassBuilder("Errors")
            .WithIsStatic();

        foreach(var errorMemberGroup in GetDistinctErrors(errorMembers).GroupBy(x => NameUtils.ToValidClassName(x.Name)))
        {
            foreach(var errorMember in errorMemberGroup)
            {
                byte[] signatureBytes = errorMember.GetSignatureBytes(out string signature);
                string errorTypeName = errorMemberGroup.Count() > 1
                    ? $"{errorMemberGroup.Key}_{HexUtils.ToHexString(signatureBytes)}"
                    : errorMemberGroup.Key;

                if(!errorTypeName.EndsWith("Error"))
                {
                    errorTypeName += "Error";
                }

                var typeBuilder = _errorTypeWriter.GenerateErrorType(errorTypeName, errorMember);

                typeBuilder.AddRawContent(
                    $$"""
                    /// <summary>
                    /// Error signature used to calculate the signature bytes.
                    /// </summary>
                    public const string Signature = "{{signature}}";
                    /// <summary>
                    /// Error signature bytes based on function signature: {{signature}}
                    /// </summary>
                    public static ReadOnlyMemory<byte> SignatureBytes { get; } 
                        = new byte[] { {{String.Join(",", signatureBytes)}} };
                    /// <summary>
                    /// Hex encoded error signature bytes based on function signature: {{signature}}
                    /// </summary>
                    public const string SignatureHex = "0x{{HexUtils.ToHexString(signatureBytes)}}";
                    """
                );



                sectionBuilder.AddInnerType(typeBuilder);
            }
        }

        interfaceBuilder.AddInnerType(sectionBuilder);
    }

    private IEnumerable<ErrorAbiMember> GetDistinctErrors(IEnumerable<ErrorAbiMember> errorMembers)
    {
        var mappedMembers = errorMembers
            .Select(x =>
            {
                _ = x.GetSignatureBytes(out string? errorSignature);
                return new
                {
                    Member = x,
                    Signature = errorSignature
                };
            })
            .ToArray();

        return mappedMembers
            .Where(x => x == mappedMembers.First(y => y.Signature == x.Signature))
            .Select(x => x.Member);
    }
}
