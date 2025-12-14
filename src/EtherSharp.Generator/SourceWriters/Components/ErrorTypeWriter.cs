using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters.Components;

public class ErrorTypeWriter
{
    private readonly FunctionBuilder _isMatchingSignatureFunction = new FunctionBuilder("IsMatchingSignature")
        .AddArgument("System.ReadOnlySpan<byte>", "errorData")
        .WithReturnType<bool>()
        .WithIsStatic()
        .AddStatement($"return errorData.Length >= 4 && SignatureBytes.Span.SequenceEqual(errorData[0..4])");

    public ClassBuilder GenerateErrorType(string errorTypeName, ErrorAbiMember errorMember)
    {
        var errorTypeBuilder = new ClassBuilder(errorTypeName)
            .WithAutoConstructor();

        var decodeMethod = new FunctionBuilder("Decode")
            .WithReturnTypeRaw(errorTypeName)
            .WithIsStatic(true)
            .AddArgument("EtherSharp.ABI.AbiDecoder", "decoder");

        var errorTypeCtorCall = new ConstructorCallBuilder(errorTypeName);

        for(int i = 0; i < errorMember.Inputs.Length; i++)
        {
            var parameter = errorMember.Inputs[i];

            if(!PrimitiveTypeWriter.TryMatchPrimitiveType(parameter.Type, out string primitiveType, out _, out string abiFunctionName, out string decodeSuffix))
            {
                throw new NotSupportedException("ABI Error can only contain primitive types");
            }

            string parameterName = NameUtils.ToValidPropertyName(parameter.Name);
            if(String.IsNullOrWhiteSpace(parameterName))
            {
                throw new NotSupportedException("ABI Error member must all have a name");
            }

            errorTypeBuilder.AddProperty(
                new PropertyBuilder(primitiveType, parameterName)
                    .WithVisibility(PropertyVisibility.Public)
                    .WithSetterVisibility(SetterVisibility.None)
            );

            string tempVarName = $"param{i}";
            decodeMethod.AddStatement($"var {tempVarName} = decoder.{abiFunctionName}(){decodeSuffix}");
            errorTypeCtorCall.AddArgument(tempVarName);
        }

        decodeMethod.AddStatement($"return {errorTypeCtorCall.ToInlineCall()}");

        errorTypeBuilder.AddFunction(decodeMethod);
        errorTypeBuilder.AddFunction(_isMatchingSignatureFunction);

        errorTypeBuilder.AddFunction(new FunctionBuilder("TryDecode")
            .AddArgument("System.ReadOnlyMemory<byte>", "errorData")
            .WithReturnType<bool>()
            .WithIsStatic()
            .AddArgument($"out {errorTypeName}", "parsedError")
            .AddStatement(
                $$"""
                if (!IsMatchingSignature(errorData.Span)) 
                {
                    parsedError = null;
                    return false;
                }

                parsedError = Decode(new EtherSharp.ABI.AbiDecoder(errorData));
                return true;
                """
            )
        );

        return errorTypeBuilder;
    }
}
