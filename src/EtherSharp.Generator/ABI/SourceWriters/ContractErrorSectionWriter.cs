using EtherSharp.Generator.ABI.Members;
using EtherSharp.Generator.ABI.SourceWriters.Components;
using EtherSharp.Generator.ABI.Util;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.ABI.SourceWriters;

internal sealed class ContractErrorSectionWriter(ErrorTypeWriter errorTypeWriter)
{
    private readonly ErrorTypeWriter _errorTypeWriter = errorTypeWriter;

    public void GenerateContractErrorSection(InterfaceBuilder interfaceBuilder, IEnumerable<ErrorAbiMember> errorMembers)
    {
        var sectionBuilder = new ClassBuilder("Errors")
            .AddBaseType("EtherSharp.Contract.Sections.IErrorsSection", true)
            .AddRawContent("private Errors() {}");

        var errorTypeNames = new List<string>();

        foreach(var errorMemberGroup in GetDistinctErrors(errorMembers).GroupBy(x => NameUtils.ToValidClassName(x.Name)))
        {
            foreach(var errorMember in errorMemberGroup)
            {
                byte[] signatureBytes = errorMember.GetSignatureBytes(out string signature);
                string errorTypeName = errorMemberGroup.Count() > 1
                    ? $"{errorMemberGroup.Key}_{HexUtils.ToHexStringUpper(signatureBytes)}"
                    : errorMemberGroup.Key;

                if(!errorTypeName.EndsWith("Error"))
                {
                    errorTypeName += "Error";
                }

                var typeBuilder = _errorTypeWriter.GenerateErrorType(errorTypeName, errorMember);
                errorTypeNames.Add(errorTypeName);

                typeBuilder.AddRawContent(
                    $$"""
                    /// <summary>
                    /// Error signature used to calculate the signature bytes.
                    /// </summary>
                    public static string ErrorSignature { get; } = "{{signature}}";
                    /// <summary>
                    /// Hex encoded error selector based on function signature: {{signature}}
                    /// </summary>
                    public static string SelectorHex { get; } = "0x{{HexUtils.ToHexStringLower(signatureBytes)}}";
                    /// <summary>
                    /// Parsed bytes4 error selector based on signature: {{signature}}
                    /// </summary>
                    public static EtherSharp.Types.Bytes4 Selector { get; } = EtherSharp.Types.Bytes4.Parse(SelectorHex);
                    """
                );

                sectionBuilder.AddInnerType(typeBuilder);
            }
        }

        var getAllSelectorsFunction = new FunctionBuilder("GetSelectors")
            .WithIsStatic(true)
            .WithVisibility(FunctionVisibility.Public)
            .WithReturnTypeRaw("EtherSharp.Types.Bytes4[]");

        getAllSelectorsFunction.AddStatement(
            $"""
                return [
            {String.Join(",\n", errorTypeNames.Select(x => $"       {x}.Selector"))}
                ]
            """
        );

        sectionBuilder.AddFunction(getAllSelectorsFunction);
        interfaceBuilder.AddInnerType(sectionBuilder);
    }

    private static IEnumerable<ErrorAbiMember> GetDistinctErrors(IEnumerable<ErrorAbiMember> errorMembers)
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
