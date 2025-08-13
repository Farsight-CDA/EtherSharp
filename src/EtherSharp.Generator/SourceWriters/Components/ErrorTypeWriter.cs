using EtherSharp.Generator.Abi.Members;
using EtherSharp.Generator.SyntaxElements;
using EtherSharp.Generator.Util;

namespace EtherSharp.Generator.SourceWriters.Components;
public class ErrorTypeWriter
{
    private readonly FunctionBuilder _isMatchingSignatureFunction = new FunctionBuilder("IsMatchingSignature")
        .AddArgument("System.ReadOnlySpan<byte>", "signature")
        .WithReturnType<bool>()
        .WithIsStatic()
        .AddStatement($"return signature.SequenceEqual(SignatureBytes.Span)");

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
            if(string.IsNullOrWhiteSpace(parameterName))
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

        return errorTypeBuilder;
    }
}
